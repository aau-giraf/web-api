#!/bin/bash

default_con="server=<db-host>;port=<db-port>;userid=<db-user>;password=<db-password>;database=<db-db>;Allow User Variables=True"
new_con="server=127.0.0.1;userid=root;password=Giraf123;database=giraf;Allow User Variables=True"
jwt_key="kfrj2fjn90f93nhf93urfg93urgb93urgdfklgnhurbn894hbg8u4brg84brg84rhg487rhg84rhng8unv8r4nv48urhf8u4hrf"
jwt_iss="AAU"
limit=2000

cp GirafRest/appsettings.template.json GirafRest/appsettings.json

sed -i "s/$default_con/$new_con/g" GirafRest/appsettings.json
sed -i "s/<jwt-key>/$jwt_key/g" GirafRest/appsettings.json
sed -i "s/<jwt-issuer>/$jwt_iss/g" GirafRest/appsettings.json
sed -i "s/\"Limit\": 20/\"Limit\": $limit/g" GirafRest/appsettings.json