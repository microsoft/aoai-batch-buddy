import logging
import azure.functions as func
import azure.durable_functions as df
import openai

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
    for job in results:
        logging.info(f"Job {i}: {job}")
        print(job)
        #await client.start_new("my_orchestrator", client_input=job)
    
    #instance_id = await client.start_new("my_orchestrator", client_input=payload)
    
    #logging.info(f"Started orchestration with ID = '{instance_id}'.")
        

@status_update.orchestration_trigger(context_name="context")
def my_orchestrator(context: df.DurableOrchestrationContext):
    return context.get_input()