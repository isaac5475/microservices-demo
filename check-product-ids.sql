SELECT op."productId", COUNT(*) as count FROM "OrderPosition" op GROUP BY op."productId" ORDER BY count DESC LIMIT 10;
