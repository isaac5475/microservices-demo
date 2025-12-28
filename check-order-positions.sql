-- Check for duplicate productIds in OrderPosition
SELECT "productId", COUNT(*) as count
FROM "OrderPosition"
GROUP BY "productId"
HAVING COUNT(*) > 1;

-- Show all OrderPositions
SELECT * FROM "OrderPosition" ORDER BY "orderId", "productId";
