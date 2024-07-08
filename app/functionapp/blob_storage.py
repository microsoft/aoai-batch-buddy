import logging
from io import BytesIO
from azure.storage.blob import BlobServiceClient



def create_blob_stream(myblob, blob_client):
    blob_stream = BytesIO()
    num_bytes = blob_client.download_blob().readinto(blob_stream)    
    logging.info(f"Downloaded {num_bytes} bytes from blob {myblob.name}")

    blob_stream.seek(0)
    return blob_stream       

def create_blob_client(connection_string, container_name, blob_name):
    blob_service_client = BlobServiceClient.from_connection_string(connection_string)
    blob_client = blob_service_client.get_blob_client(container_name, blob_name)
    return blob_client


