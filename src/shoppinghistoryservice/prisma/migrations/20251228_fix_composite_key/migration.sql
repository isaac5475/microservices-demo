-- Drop the incorrect primary key and recreate with composite key
ALTER TABLE "OrderPosition" DROP CONSTRAINT "OrderPosition_pkey";
ALTER TABLE "OrderPosition" ADD CONSTRAINT "OrderPosition_pkey" PRIMARY KEY ("productId", "orderId");
