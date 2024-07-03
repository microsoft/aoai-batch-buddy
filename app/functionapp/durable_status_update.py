import logging
import azure.functions as func
import azure.durable_functions as df
import openai
import json
import os
from azure.storage.blob import BlobServiceClient

status_update = df.Blueprint()


@status_update.timer_trigger(arg_name="req", schedule="0 0 */1 * * *", run_on_startup=True) # Every hour
@status_update.durable_client_input(client_name="client")
async def start_orchestrator(req: func.TimerRequest, client):
    # Get list of batch jobs and send to orchestrator for status update
    batch_client = openai.AzureOpenAI(
        api_version="2024-04-15-preview",
        azure_deployment="gpt-4o-batch"
    )
    
    results = batch_client.batches.list(limit=10)
    payload = json.dumps(results.to_dict()["value"])
    
    instance_id = await client.start_new("my_orchestrator", client_input=payload)
    
    #logging.info(f"Started orchestration with ID = '{instance_id}'.")
        

@status_update.orchestration_trigger(context_name="context")
def my_orchestrator(context: df.DurableOrchestrationContext):
    input = context.get_input()
    jobs = json.loads(input)
    
    # fan out - start a new activity function for each job
    tasks = []
    for j in jobs:
        tasks.append(context.call_activity(name="update_status", input_=json.dumps(j)))
    
    status_results = yield context.task_all(tasks)
    
    return status_results

@status_update.activity_trigger("jobstr", "update_status")
def update_status(jobstr: str):
    # Update the status of the job
    job = json.loads(jobstr)
    logging.info(f"Updating status for job {job['id']}")
    print(job['status'])
    
    # if job is complete, store the output file in blob storage
    if job["status"] == "Completed":
        # get the file from openai
        file_client = openai.AzureOpenAI(
        api_version="2024-04-15-preview"
    )
        file = file_client.files.content(job["output_file_id"])
        conn_str = os.environ["BlobStorageConnectionString"]
        # store the output file in blob storage
        blob_client = BlobServiceClient.from_connection_string(conn_str)
        container_client = blob_client.get_container_client("outputfiles")        
            
        container_client.upload_blob(name=f"{job['output_file_id']}.jsonl", data=file.content, overwrite=True)
    
    # give cosmos the 411
    
    return job