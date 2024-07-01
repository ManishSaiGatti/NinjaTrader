using Newtonsoft.Json;
using NinjaTrader.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using static SimpleStrategy.Strategy;

namespace SimpleStrategy
{
    public class Strategy
    {
        private static Client client;
        private static string instrument;
        private static double buyPrice;
        private static double sellPrice;
        private static string account;

        public class Order
        {
            public string Account { get; set; }
            public string Instrument { get; set; }
            public string placeOrCancel { get; set; }
            public string buyOrSell { get; set; }
            public int Qty { get; set; }
            public string OrderType { get; set; }
            public double LimitPrice { get; set; }
            public double StopPrice { get; set; }
            public string TIF { get; set; }
            public string OrderID { get; set; }
        }

        public class OrdersList
        {
            public List<Order> orders { get; set; }
        }
        
        public class orderDetails
        {
            public string command;
            public string instrument;
            public string action;
            public int qty;
            public string orderType;
            public double limitPrice;
            public double stopPrice;
            public string tif;
            public string orderID;

            public orderDetails(string command, string instrument, string action, int qty, string orderType, 
                double limitPrice, double stopPrice)
            {
                this.instrument = instrument;
                this.qty = qty;
                this.limitPrice= limitPrice;
                this.stopPrice = stopPrice;
// -------------------------------------------------------------------------------------------------------
                if (command.ToLower().StartsWith("p"))
                {
                    this.command = "PLACE";
                }
                else if (command.ToLower().StartsWith("c"))
                {
                    this.command = "CANCELALLORDERS";
                    this.action = "";
                    this.orderType = "";
                    this.tif = "";
                    this.orderID = "";
                    return;
                }
                else
                {
                    Console.WriteLine("ERROR");
                }
// -------------------------------------------------------------------------------------------------------
                if (action.ToLower().StartsWith("b"))
                {
                    this.action = "BUY";
                }
                else if (action.ToLower().StartsWith("s"))
                {
                    this.action = "SELL";
                }
// -------------------------------------------------------------------------------------------------------
                if (orderType.ToLower().Equals("m"))
                {
                    this.orderType = "MARKET";
                }
                else if (orderType.ToLower().Equals("l"))
                {
                    this.orderType = "LIMIT";
                }
                else if (orderType.ToLower().Equals("sm"))
                {
                    this.orderType = "STOPMARKET";
                }
                else if (orderType.ToLower().Equals("sl"))
                {
                    this.orderType = "STOPLIMIT";
                }
                else
                {
                    Console.WriteLine("ERROR");
                }
// -------------------------------------------------------------------------------------------------------
                if (this.orderType == "MARKET")
                {
                    this.limitPrice = 0;
                    this.stopPrice = 0;
                }
                else if (this.orderType == "LIMIT")
                {
                    this.stopPrice = 0;
                    if (limitPrice == 0)
                    {
                        // throw error
                        Console.WriteLine("ERROR: No limit price (LIMIT)");
                    }
                    this.limitPrice = limitPrice;
                }
                else if (this.orderType == "STOPMARKET")
                {
                    this.limitPrice = 0;
                    if (stopPrice == 0)
                    {
                        // throw error
                        Console.WriteLine("ERROR: No stop price (STOPMARKET)");
                    }
                    this.stopPrice = stopPrice;
                }
                else if (this.orderType == "STOPLIMIT")
                {
                    if (limitPrice == 0 || stopPrice == 0)
                    {
                        // throw error
                        Console.WriteLine("ERROR: No stop or limit price (STOPLIMIT)");
                    }
                    this.limitPrice = limitPrice;
                    this.stopPrice = stopPrice;
                }
            }
        }

        static void Main(string[] args)
        {
            client = new Client();
            //instrument = "ES JUN24";
            //buyPrice = 5500;
            //sellPrice = 110;
            //account = "Sim101";

            string orderID = client.NewOrderId();
            //string orderID = "1234";
            Console.WriteLine("OrderID: " + orderID);
            //return;

            Console.WriteLine("Hello");

            string host = "192.168.0.79"; //windows
            //string host = "172.16.253.128"; //MAC (Windows VM)
            //subnet IP: 172.16.135.0
            //string host = "172.16.135.0";
            //string host = "192.168.0.146";
            int port = 36973;
            Console.WriteLine("Connection: " + client.SetUp(host, port));

            Console.WriteLine("Connected: " + client.Connected(1));

            //client.UnsubscribeMarketData(instrument);
            //client.SubscribeMarketData(instrument);

            //Console.WriteLine("cash value: " + client.CashValue(account));
            //Console.WriteLine("buying power: " + client.BuyingPower(account));
            //Console.WriteLine(client.Orders(account));

            Console.WriteLine("Getting prices");
            //double bid = 0;
            //double ask = 0;

            executeStrategy(orderID);
            //Console.WriteLine(client.ConfirmOrders(1));
            //string orders = client.Orders(account);
           // Console.WriteLine("Order Status: " + orders);
            //string[] orderList = orders.Split('|');
            /*for(int i = 0; i <  orderList.Length; i++)
            {
                Console.WriteLine(client.OrderStatus(orderList[i]));
            }

            client.Orders(account);*/

            client.TearDown();
        }

        static void executeStrategy(string orderID)
        {
            //int marketPosition = client.MarketPosition(instrument, account);

            string OCOID = "";
            string strategy = null;
            string strategyID = null;
            string tif = "day";

            string json = File.ReadAllText(".\\..\\..\\..\\values.json");
            OrdersList ordersList = JsonConvert.DeserializeObject<OrdersList>(json);

            foreach (var order in ordersList.orders)
            {
                /*string command = order.Command;
                string action = order.Action;
                int qty = order.Qty;
                string orderType = order.OrderType;
                double limitPrice = order.LimitPrice;
                double stopPrice = order.StopPrice;
                string tif = order.TIF;
                orderID = order.OrderID;*/
                Console.WriteLine(order.Account);
                Console.WriteLine(order.placeOrCancel);
                Console.WriteLine(order.Instrument);
                Console.WriteLine(order.buyOrSell);
                orderDetails orderDets = new orderDetails(order.placeOrCancel, order.Instrument, 
                    order.buyOrSell, order.Qty, order.OrderType, order.LimitPrice, order.StopPrice);
                string[] accounts = order.Account.Split(',');
                foreach (string account in accounts)
                {
                    /*Console.WriteLine(account);
                    Console.WriteLine("    " + orderDets.instrument);
                    Console.WriteLine("    " + orderDets.command);
                    Console.WriteLine("    " + orderDets.action);
                    Console.WriteLine("    " + orderDets.qty);
                    Console.WriteLine("    " + orderDets.orderType);
                    Console.WriteLine("    " + orderDets.limitPrice);
                    Console.WriteLine("    " + orderDets.stopPrice);
                    Console.WriteLine("    " + orderDets.tif);
                    Console.WriteLine("    " + orderDets.orderID);
                    Console.WriteLine();*/
                    
                    Console.WriteLine(client.Command(orderDets.command, account, orderDets.instrument, 
                        orderDets.action, orderDets.qty, orderDets.orderType, orderDets.limitPrice, 
                        orderDets.stopPrice, tif, OCOID, orderID, strategy, strategyID));
                }
                //Console.WriteLine("------------------------------------------------------------");
            }

            //Console.WriteLine("mp: " + marketPosition);
            //Console.WriteLine("ask: " + ask);
            //Console.WriteLine("bid: " + bid);
        }
    }
}