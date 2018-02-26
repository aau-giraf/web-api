#  -f [path/to/dockerfile] -t [containerTag] . 
docker build -f GirafRest.Test/Dockerfile -t giraf-rest-test .
docker build -f GirafRest/Dockerfile -t giraf-rest .