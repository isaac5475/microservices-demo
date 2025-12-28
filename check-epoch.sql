SELECT id, "placedOn", EXTRACT(EPOCH FROM "placedOn") as seconds FROM "Order" LIMIT 3;
