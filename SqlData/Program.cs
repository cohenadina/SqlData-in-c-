using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace SqlData
{
    class Program
    {
        static void Main(string[] args)
        {


            Console.WriteLine("Please enter a orderId :");
            int orderid = int.Parse(Console.ReadLine());
            Console.WriteLine("Please choose a number: 1-to read , 2-to insert, 3-to update, 4-to delete : ");
            int num = int.Parse(Console.ReadLine());

            using (SqlConnection conn = new SqlConnection(@"Data Source=.\SQLEXPRESS; Initial Catalog=Northwind; Integrated Security=SSPI"))
            {
                DataTable order = new DataTable();
                SqlDataAdapter adapter = CreateCustomAdapter(conn);
                SqlCommand cmd = new SqlCommand("select OrderId, o.ProductID, p.ProductName, o.UnitPrice, Quantity,Discount  from [Order Details] o join products p on p.ProductId=o.ProductId where OrderId=" + orderid, conn);
                adapter.SelectCommand = cmd;
                adapter.Fill(order);




                switch (num)
                {
                    case 1:
                        PrintTable(order);
                        break;
                    case 2:
                        Console.WriteLine("Please enter the productName :");
                        string productname = Console.ReadLine();
                        Console.WriteLine("Please enter the quantity :");
                        int quantity = int.Parse(Console.ReadLine());
                        DataRow neworder = order.NewRow();
                        neworder["OrderId"] = orderid;
                        neworder["ProductName"] = productname;
                        neworder["quantity"] = quantity;
                        order.Rows.Add(neworder);
                        PrintTable(order);
                        break;

                    case 3:
                        Console.WriteLine("Please enter the product id :");
                        int productId = int.Parse(Console.ReadLine());
                        Console.WriteLine("Please enter the quantity you want to change :");
                        int qtt = int.Parse(Console.ReadLine());
                        DataColumn[] pk = { order.Columns["OrderId"], order.Columns["ProductId"] };
                        order.PrimaryKey = pk;
                        object[] pkvalues = { orderid, productId };
                        DataRow found = order.Rows.Find(pkvalues);
                        found[4] = qtt;
                        adapter.Update(order);
                        PrintTable(order);

                        break;
                    case 4:
                        Console.WriteLine("Please enter the productId to delete:");
                        int productid1 = int.Parse(Console.ReadLine());
                        DataColumn[] pk1 = { order.Columns["ProductId"] };
                        order.PrimaryKey = pk1;
                        object[] pkval = { productid1 };
                        DataRow found1 = order.Rows.Find(pkval);
                        found1.Delete();
                        adapter.Update(order);
                        PrintTable(order);
                        break;
                }

            }

            Console.ReadKey();


        }

        static SqlDataAdapter CreateCustomAdapter(SqlConnection conn)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();


            adapter.InsertCommand = new SqlCommand("insert into [Order Details] values(OrderID,ProductId, Quantity) select(@orderId, @productid, @quantity) from [order details] o join Products p on o.ProductID = p.ProductID where p.ProductName = @productname ) ", conn);
            adapter.InsertCommand.Parameters.Add("@OrderID", SqlDbType.Int, 5, "OrderID");
            adapter.InsertCommand.Parameters.Add("@productname", SqlDbType.VarChar, 50, "productname");
            adapter.InsertCommand.Parameters.Add("@ProductID", SqlDbType.Int, 2, "ProductID");
            adapter.InsertCommand.Parameters.Add("@Quantity", SqlDbType.SmallInt, 2, "Quantity");



            adapter.UpdateCommand = new SqlCommand("update [Order Details] set quantity = @quantity where OrderId = @orderId and ProductId = @productid", conn);
            adapter.UpdateCommand.Parameters.Add("@orderId", SqlDbType.Int, 5, "orderid");
            adapter.UpdateCommand.Parameters.Add("@quantity", SqlDbType.Money, 4, "quantity");
            adapter.UpdateCommand.Parameters.Add("@productid", SqlDbType.Int, 5, "productid");



            adapter.DeleteCommand = new SqlCommand("Delete from [Order Details] where OrderId= @orderId and ProductID=@productid", conn);
            SqlParameter param = new SqlParameter("@orderId", SqlDbType.Int, 5, "orderId");
            adapter.DeleteCommand.Parameters.Add("@productid", SqlDbType.Int, 5, "productid");
            param.SourceVersion = DataRowVersion.Original;
            adapter.DeleteCommand.Parameters.Add(param);


            return adapter;
        }

        static void PrintTable(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                Console.WriteLine();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    Console.Write(dr[i] + "\t");
                }
            }
        }
    }
}



