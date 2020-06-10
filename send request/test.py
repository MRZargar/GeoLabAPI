from pgDB import pgDB
from GeoLabAPI import GeoLabAPI

API = GeoLabAPI()
# DB = pgDB("localhost","geolab","zargar", "Z@rgar76")

# datas = DB.getQuery("select * from station2;")

try:
    API.send_health_status('station2', 2)
except Exception as ex:
    print(ex)