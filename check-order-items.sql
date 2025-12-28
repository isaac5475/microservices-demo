SELECT o."id", o."createdAt", COUNT(op."productId") as item_count FROM "Order" o LEFT JOIN "OrderPosition" op ON o."id" = op."orderId" GROUP BY o."id" ORDER BY o."createdAt" DESC;
