#!/bin/bash
set -e

echo "Executing init-db.sh script"

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE DATABASE $DB_NAME;
    CREATE USER $DB_USER WITH PASSWORD '$DB_PASSWORD';
    GRANT ALL PRIVILEGES ON DATABASE $DB_NAME TO $DB_USER;
	ALTER USER $DB_USER WITH SUPERUSER;
EOSQL

for DB in "$DB_NAME"; do
	echo "Loading Postgis extensions into $DB database"
	psql -v ON_ERROR_STOP=1 --dbname "$DB" <<-'EOSQL'
		CREATE EXTENSION IF NOT EXISTS postgis;
		CREATE EXTENSION IF NOT EXISTS postgis_topology;
		CREATE EXTENSION IF NOT EXISTS fuzzystrmatch;
		CREATE EXTENSION IF NOT EXISTS postgis_tiger_geocoder;
	EOSQL
done