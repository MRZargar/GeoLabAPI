# **GeoLabAPI Document**

# 1. Histogram

## Get Histogram Values

> `GET: /api/Data/Histogram/{tableName}?week={}&t={}`

# 2. Data

## Send Data

> `POST: /api/Data/{tableName}`

```json
body:
{
    "week": 1234,
    "t": 123456,
    "a_x": 12.34,
    "a_y": 12.34,
    "a_z": 12.34,
    "temp": 12.34
}
```

**Note:** _You must send the data as array_

## Get Data

if you want get a **range** of Datas:

> `GET: /api/Data/{tableName}`
>
> `GET: /api/Data/{tableName}?toWeek={}&toT={}`
>
> `GET: /api/Data/{tableName}?fromWeek={}&fromT={}`
>
> `GET: /api/Data/{tableName}?fromWeek={}&fromT={}&toWeek={}&toT={}`

if you want get **a** data:

> `GET: /api/Data/{tableName}/{week}/{t}`

## Update Data

> `PUT: /api/Data/{tableName}/{week}/{t}`

```json
body:
{
    "week": 2110,
    "t": 846000,
    "temp": 23.58
}
```

**Note:** _GPS time is required_

## Delete Data

> `DELETE: /api/Data/{tableName}/{week}/{t}`

# 3. Raspberry

## Submit Raspderry

> `POST: /api/Data/{RaspberryID}/313`

**Notes:**

1. _If Raspberry is already registered, nothing will happen_
2. body is empty

## Get Table Name related by Raspberry

> `GET: /api/Data/{RaspberryID}/313`

## Set Raspberry Status

> `PUT: /api/Data/{tableName}`

```json
body:
{
    "healthCode": 1
}
```

# **Examples**

```python
import requests

r = requests.get(url, verify=False)

if r.status_code not in range(200,300):
    raise Exception(r.status_code)

return r.text
```

```python
import json as JSON

json = '''
[
    {
        "week": 1234,
        "t": 123456.0,
        "a_x": 12.34,
        "a_y": 12.34,
        "a_z": 12.34,
        "temp": 12.34
    },
    {
        "week": 1234,
        "t": 123456.1,
        "a_x": 12.34,
        "a_y": 12.34,
        "a_z": 12.34,
        "temp": 12.34
    }
]
'''

json = JSON.loads(json)

r = requests.post(url, json=json, verify=False)

if r.status_code not in range(200,300):
    raise Exception(r.status_code)
```