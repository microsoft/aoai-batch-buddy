import logging
from openai import AzureOpenAI

def create_batch_client(api_key:str, api_version:str, azure_endpoint:str):
    client = AzureOpenAI(
        api_key=api_key,  
        api_version=api_version,
        azure_endpoint=azure_endpoint
    )
    return client

def create_batch_job(client, batch_input_file, description):
    batch_job = client.batches.create(
        input_file_id=batch_input_file.id,
        endpoint="/v1/chat/completions",
        completion_window="24h",
        metadata={
            "description": f"{description}",
        }
    )
    logging.info(f"Batch input file created: {batch_input_file.id}") 
    return batch_job