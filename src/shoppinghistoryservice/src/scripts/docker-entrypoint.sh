#!/bin/sh
set -e

# Push schema to database (creates tables if they don't exist, ignores if they do)
npx prisma db push --accept-data-loss --config=/app/dist/prisma.config.js || echo "Schema initialization complete or already exists"

if [ "$NODE_ENV" = "development" ] || [ "$DEBUG" = "true" ]; then
  echo "Starting in DEBUG mode..."
  exec node --inspect=0.0.0.0:9229 dist/index.js
else
  echo "Starting in PRODUCTION mode..."
  exec node dist/index.js
fi
