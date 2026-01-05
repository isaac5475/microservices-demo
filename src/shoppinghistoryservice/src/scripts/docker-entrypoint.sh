#!/bin/sh
set -e

# Wait for database to be ready with retry logic
MAX_RETRIES=30
RETRY_DELAY=1

echo "Waiting for database to be ready..."
for i in $(seq 1 $MAX_RETRIES); do
  if npx prisma db push --accept-data-loss --config=/app/dist/prisma.config.js; then
    echo "Database is ready and schema synchronized"
    break
  else
    echo "Database connection attempt $i/$MAX_RETRIES failed"
    if [ $i -lt $MAX_RETRIES ]; then
      echo "Waiting ${RETRY_DELAY}s before retry..."
      sleep $RETRY_DELAY
    else
      echo "Max retries reached. Database is not available."
      exit 1
    fi
  fi
done

if [ "$NODE_ENV" = "development" ] || [ "$DEBUG" = "true" ]; then
  echo "Starting in DEBUG mode..."
  exec node --inspect=0.0.0.0:9229 dist/index.js
else
  echo "Starting in PRODUCTION mode..."
  exec node dist/index.js
fi
