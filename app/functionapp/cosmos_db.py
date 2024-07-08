import logging
import datetime
from azure.cosmos import PartitionKey
from azure.cosmos.aio import CosmosClient, ContainerProxy

async def upsert_file_to_cosmos(submitted_file, cosmos_container:ContainerProxy, token_count:int=0):    

    await cosmos_container.upsert_item(
        dict(id=submitted_file.id, 
             file_name=submitted_file.filename, 
             status=submitted_file.status,
             status_details=submitted_file.status_details,              
             created_date= submitted_file.created_at,              
             purpose=submitted_file.purpose,
             token_count=token_count)
    )

async def cosmosdb_file_id_exists(cosmos_container:ContainerProxy, fileName:str)->bool:
    
    query = f'SELECT * FROM c WHERE c.file_name=@name'
    parameters = [{"name": "@name", "value": fileName}]
    
    results = cosmos_container.query_items(query=query,             
            parameters=parameters
            )

    # item_list = [item async for item in results]    
    item_list = []
    async for item in results:
        item_list.append(item)

    if len(item_list) > 0:
        logging.info(f"File {fileName} has already been processed.")
        return True
        
    return False

async def get_cosmos_container(cosmos_client:CosmosClient, database_name:str, container_name:str, partition_id="id")-> ContainerProxy:
    database = await cosmos_client.create_database_if_not_exists(id=database_name)
    partition_key=PartitionKey(path=f"/{partition_id}")
    container:ContainerProxy= await database.create_container_if_not_exists(container_name, partition_key=partition_key)
    return container