-- Delete orders with no items
DELETE FROM "Order" WHERE id IN (
  SELECT o.id FROM "Order" o 
  LEFT JOIN "OrderPosition" op ON o.id = op."orderId" 
  WHERE op."orderId" IS NULL
);
