using System;
using System.Threading;
using System.Collections.Concurrent;
//using Microsoft.ConcurrencyVisualizer.Instrumentation;

namespace barbero_duermiente
{
    class Program
    {
        internal static AutoResetEvent ClienteEvent = new AutoResetEvent(false);
        internal static ConcurrentQueue<Cliente> queue = new ConcurrentQueue<Cliente>();

        static void Main(string[] args)
        {
            Random rand = new Random();

            new Thread(Barber.CortarCabello) { IsBackground = true, Name = "Barbero" }.Start();

            Thread.Sleep(100);

            Thread.CurrentThread.Name = "Main";

            for (int i = 0; i < 11; i++)
            {
                int temp = i;
                Thread.Sleep(rand.Next(600, 2000));
                Cliente c = new Cliente() { Name = "cliente Num.- " + temp };
                queue.Enqueue(c);

                if (queue.Count == 1)
                {
                    Cliente.LevantarBarbero();
                }

                Thread.Sleep(1500);
            }

            Console.WriteLine("Fin del programa...");
            Console.ReadKey();
        }
    }

    class Cliente
    {
        public string Name { get; set; }

        internal static void LevantarBarbero()
        {
            Program.ClienteEvent.Set();
        }
    }

    class Barber
    {
        internal static void CortarCabello()
        {
            while (!Program.queue.IsEmpty)
            {
                Cliente c;
                Thread.Sleep(1000);

                if (Program.queue.TryDequeue(out c))
                {
                    Console.WriteLine("Okey, el cabello se le corto al {0}", c.Name);
                }
                else
                {
                    Console.WriteLine("El cliente no puede salir aun...");
                }
            }

            Console.WriteLine("El barbero se va a DORMIR");
            Dormir();
        }

        private static void Dormir()
        {
            //using (Markers.EnterSpan("wait start"))
            //{
                Program.ClienteEvent.WaitOne();
                Console.WriteLine("Levantate barbero, un cliente llego...");
            //}
            CortarCabello();
        }
    }
}
