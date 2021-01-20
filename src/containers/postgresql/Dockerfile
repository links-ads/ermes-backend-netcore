FROM postgis/postgis:12-master
EXPOSE 5432

RUN apt-get update

COPY ./src/containers/postgresql/postgis.sh /docker-entrypoint-initdb.d/init-db.sh