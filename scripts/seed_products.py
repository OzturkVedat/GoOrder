import boto3
from botocore.exceptions import ClientError

dynamodb = boto3.resource("dynamodb", region_name="eu-north-1")
table = dynamodb.Table("GoOrderTable")

seed_check_pk = "STORE#S1"
seed_check_sk = "DETAILS"


def is_already_seeded():
    try:
        response = table.get_item(Key={"PK": seed_check_pk, "SK": seed_check_sk})
        return "Item" in response
    except ClientError as e:
        print(f"Error checking seed status: {e.response['Error']['Message']}")
        return False


if is_already_seeded():
    print("Seed already exists. Skipping seeding.")
else:
    stores = [
        {
            "StoreId": "S1",
            "Name": "Tech Haven",
            "Location": "Nova City - Silicon District",
        },
        {
            "StoreId": "S2",
            "Name": "Fashion Hub",
            "Location": "Aurora Town - Velvet Ward",
        },
    ]

    products = [
        {
            "StoreId": "S1",
            "ProductId": "P100",
            "Name": "Gaming Laptop",
            "Price": 1200,
            "Stock": 4,
            "Reserved": 0,
            "Category": "Electronics",
        },
        {
            "StoreId": "S1",
            "ProductId": "P101",
            "Name": "Mechanical Keyboard",
            "Price": 150,
            "Stock": 11,
            "Reserved": 0,
            "Category": "Electronics",
        },
        {
            "StoreId": "S2",
            "ProductId": "P200",
            "Name": "Running Shoes",
            "Price": 80,
            "Stock": 17,
            "Reserved": 0,
            "Category": "Shoes",
        },
        {
            "StoreId": "S2",
            "ProductId": "P201",
            "Name": "Denim Jacket",
            "Price": 50,
            "Stock": 21,
            "Reserved": 0,
            "Category": "Clothing",
        },
    ]

    try:
        with table.batch_writer() as batch:
            # seed stores
            for store in stores:
                batch.put_item(
                    Item={
                        "PK": f"STORE#{store['StoreId']}",
                        "SK": "DETAILS",
                        "EntityType": "Store",
                        "Name": store["Name"],
                        "Location": store["Location"],
                    }
                )

            # seed products
            for product in products:
                batch.put_item(
                    Item={
                        "PK": f"STORE#{product['StoreId']}",
                        "SK": f"PRODUCT#{product['ProductId']}",
                        "EntityType": "Product",
                        "Name": product["Name"],
                        "Price": product["Price"],
                        "Stock": product["Stock"],
                        "Category": product["Category"],
                        "GSI1PK": f"STORE#{product['StoreId']}#CATEGORY#{product['Category']}",
                        "GSI1SK": f"PRODUCT#{product['ProductId']}",
                    }
                )
        print("Seeded stores and products successfully!")
    except ClientError as e:
        print(f"Failed to seed data: {e.response['Error']['Message']}")
