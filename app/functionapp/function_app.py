import os
import datetime
import azure.functions as func
import logging
from durable_status_update import status_update
# from io import BytesIO
# from openai import AzureOpenAI
# from azure.storage.blob import BlobServiceClient
# from azure.cosmos import PartitionKey
from token_count import TokenCount
from azure.cosmos.aio import CosmosClient, ContainerProxy

from blob_storage import create_blob_stream, create_blob_client
from cosmos_db import upsert_file_to_cosmos, get_cosmos_container, cosmosdb_file_id_exists
from aoai_batch import create_batch_job, create_batch_client


app = func.FunctionApp()
app.register_functions(status_update)

api_key=os.getenv("AZURE_OPENAI_API_KEY")
api_version=os.getenv("AZURE_OPENAI_API_VERSION")
azure_endpoint=os.getenv("AZURE_OPENAI_ENDPOINT")
connection_string=os.getenv("BLOB_STORAGE_CONNECTION_STRING")
cosmos_uri=os.getenv("COSMOSDB_ACCOUNT_URI")
cosmos_key=os.getenv("COSMOSDB_ACCOUNT_KEY")
cosmos_container_name=os.getenv("COSMOSDB_CONTAINER_NAME")
cosmos_db_name=os.getenv("COSMOSDB_DATABASE_NAME")



@app.route(route="http_trigger", auth_level=func.AuthLevel.ANONYMOUS)
def http_trigger(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')

    name = req.params.get('name')
    if not name:
        try:
            req_body = req.get_json()
        except ValueError:
            pass
        else:
            name = req_body.get('name')

    if name:
        return func.HttpResponse(f"Hello, {name}. This HTTP triggered function executed successfully.")
    else:
        return func.HttpResponse(
            "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response.",
            status_code=200
        )


@app.blob_trigger(arg_name="myblob", path="batch-files",
                connection="AzureWebJobsStorage") 
async def batch_submitted(myblob: func.InputStream):
    logging.info(f"Python blob trigger function processed blob"
                f"Name: {myblob.name}"
                f"Blob Size: {myblob.length} bytes"
                )
    
    container_name = myblob.name.split("/")[0]
    blob_name = myblob.name.split("/")[1]
    
    cosmos_client = CosmosClient(cosmos_uri, cosmos_key)
    
    cosmos_container:ContainerProxy = await get_cosmos_container(cosmos_client, cosmos_db_name, cosmos_container_name)    
    
    #Function is triggered when a tag is added. 
    #Check if file already exists in CosmosDB and terminate function if it does
    if await cosmosdb_file_id_exists(cosmos_container, blob_name):
        await cosmos_client.close()
        return

    file_client = create_batch_client(api_key, api_version, azure_endpoint)
    blob_client = create_blob_client(connection_string, container_name, blob_name)
    blob_stream = create_blob_stream(myblob, blob_client)

    tc=TokenCount(model_name="gpt-4o")
    # myblob.seek(0)
    token_count = tc.num_tokens_from_string(myblob.read().decode("utf-8"))
    
    try:        
        batch_input_file = file_client.files.create(
            file=(blob_name, blob_stream.getvalue()),
            purpose="batch"
        )
    except Exception as e:
        logging.error(f"Error creating batch input file: {e}. File Name: {blob_name}. Date: {datetime.datetime.now()}")
        return
    
    file_status=batch_input_file.status

    while file_status !="processed":
        submitted_file = file_client.files.wait_for_processing(batch_input_file.id) #Default timeout=30min polling rate:5secs
        file_status = submitted_file.status

    await upsert_file_to_cosmos(submitted_file, cosmos_container, token_count)

    await cosmos_client.close()

    #tag the file as processed in the blob storage
    blob_client.set_blob_tags(tags={"file_id": submitted_file.id, "submitted_date": datetime.datetime.now().isoformat()})

    logging.info(f"File {blob_name} has been processed and stored in CosmosDB id:{submitted_file.id}")