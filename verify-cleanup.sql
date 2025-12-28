SELECT COUNT(*) as total_orders FROM "Order";
SELECT COUNT(DISTINCT "orderId") as orders_with_items FROM "OrderPosition";
