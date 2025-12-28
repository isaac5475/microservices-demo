SELECT o.id, o."placedOn", COUNT(op."productId") as item_count 
FROM "Order" o 
LEFT JOIN "OrderPosition" op ON o.id = op."orderId" 
GROUP BY o.id, o."placedOn" 
ORDER BY o."placedOn" DESC;

SELECT op."orderId", op."productId"
FROM "OrderPosition" op
ORDER BY op."orderId" DESC;
