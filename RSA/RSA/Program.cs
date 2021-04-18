using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RSA
{
    class Program
    {
        public static int l = 1024;
        public static  BigInteger e = 65537;
        public static BigInteger d;
        public static BigInteger p;
        public static BigInteger q;
        public static BigInteger N;
        public static BigInteger F;

        //генерация BigInteger заданной длины
        public static BigInteger RandomIntegerBelow(BigInteger N)
        {
            byte[] bytes = N.ToByteArray();
            BigInteger R;
            Random random = new Random();
            do
            {
                random.NextBytes(bytes);
                bytes[bytes.Length - 1] &= (byte)0x7F; //force sign bit to positive
                R = new BigInteger(bytes);
            } while (R >= N);

            return R;
        }

        public static string ToBinaryString( BigInteger bigint)
        {
            var bytes = bigint.ToByteArray();
            var index = bytes.Length - 1;
            var base2 = new StringBuilder(bytes.Length * 8);
            var binary = Convert.ToString(bytes[index], 2);
            if (binary[0] != '0' && bigint.Sign == 1) base2.Append('0');
            base2.Append(binary);
            for (index--; index >= 0; index--)
                base2.Append(Convert.ToString(bytes[index], 2).PadLeft(8, '0'));
            return base2.ToString();
        }
        

        // слева направо
        public static BigInteger BinPow(BigInteger a, BigInteger k)
        {
            BigInteger u = new BigInteger(1);
            string str = ToBinaryString(k);
            for (int i= 0; i < str.Length; i++)
            {
                u = u * u;
                if (Int32.Parse(str[i].ToString()) != 0)
                {
                    u = u * a;
                }
            }
            return u;
        }

        public static BigInteger BinPowModul(BigInteger a, BigInteger k, BigInteger n)
        {
            BigInteger u = new BigInteger(1);
            string str = ToBinaryString(k);
            for (int i = 0; i < str.Length; i++)
            {
                u = u * u;
                u = u % n;
                if (Int32.Parse(str[i].ToString()) != 0)
                {
                    u = u * a;
                    u = u % n;
                }
            }
            return u;
        }

        public static BigInteger NOD(BigInteger a, BigInteger b)
        {
            while ( a.CompareTo(0)!=0 && b.CompareTo(0) != 0)
            {
                if (a.CompareTo(b) == 1)
                    a = a % b;
                else
                    b = b % a;
            }

            if (a.CompareTo(0) == 1) return a;
            else return b;
        }

        // тест Ферма
        public static bool IsPrime(BigInteger n)
        {
            Random rnd = new Random();
            BigInteger a = 0;
            do
            {
                a = RandomIntegerBelow(n);
            }
            while (a >= n || a < 0);

            if ( NOD(a, n).CompareTo(1) != 0 )
                return false;

            if (BinPowModul(a, n - 1, n).CompareTo(1) != 0 ) 
                return false;
             
            return true;
        }
        public static BigInteger GenPrimes(int l)
        {
            BigInteger p = 0 ;
            int i = 0;
            BigInteger n = 0;
            while (i!=100)
            {
                Random rnd = new Random();
                BigInteger add = RandomIntegerBelow(BinPow(2, l - 1));
                while (BigInteger.Remainder(add,2).CompareTo(1) != 0)
                {
                    add = RandomIntegerBelow(BinPow(2, l - 1));
                }
                n = BigInteger.Add(BinPow(2, l - 1), add);
                for ( i = 0; i < 100; i++)
                {
                    if (!IsPrime(n)) break;
                }
            }
            p = n;
            return p;
        }

        // нахождение обратного
        public static BigInteger Euclid(BigInteger a, BigInteger b)
        {
            BigInteger N = b;
            if (a.CompareTo(0) == -1) a = a + N;
            if (NOD(a, b) == 1)
            {
                (BigInteger, BigInteger, BigInteger) r = (0, a, b);
                (BigInteger, BigInteger, BigInteger) x = (0, 1, 0);
                (BigInteger, BigInteger, BigInteger) y = (0, 0, 1);

                while (r.Item3.CompareTo(0) != 0)
                {
                    r = (r.Item2, r.Item3, 0);
                    x = (x.Item2, x.Item3, 0);
                    y = (y.Item2, y.Item3, 0);
                    BigInteger q = BigInteger.Abs(r.Item1 / r.Item2);
                    r.Item3 = r.Item1 - q * r.Item2;
                    x.Item3 = x.Item1 - q * x.Item2;
                    y.Item3 = y.Item1 - q * y.Item2;
                }

                return (x.Item2.CompareTo(0) == 1) ? x.Item2 : (x.Item2 + N);
            }
            else
            {
                Console.WriteLine("Нельзя вычиcлить обратное!");
                return -1;
            }
        }

        public static void Gen(int l)
        {
            p = GenPrimes(l);
            while (NOD(e, p - 1).CompareTo(1) != 0)
            {
                p = GenPrimes(l);
            }
            Console.WriteLine("p = " + p);

            do
            {
                q = GenPrimes(l);
                while (NOD(e, q-1).CompareTo(1) != 0)
                {
                    q = GenPrimes(l);
                }
            } while (p.CompareTo(q) == 0);

            Console.WriteLine("q = " + q);

            Console.WriteLine("e = " + e);

            N = BigInteger.Multiply(p, q);
            Console.WriteLine("n = " + N);

            F = BigInteger.Multiply(BigInteger.Add(q, -1), BigInteger.Add(p, -1));
            Console.WriteLine("F = " + F);

             d = Euclid(e, F);
            Console.WriteLine("d = " + d);
            Console.WriteLine("Проверка");
            BigInteger prov = BigInteger.Multiply(d, e);
            Console.WriteLine("{0} * {1} = {2}, " + '\n' + "  {2} mod {3} = {4}", d, e, prov, F, BigInteger.Remainder(prov, F));

        }

        public static void GenTest(int l, BigInteger e1, BigInteger p1, BigInteger q1)
        {
            p = p1;
            Console.WriteLine("p = " + p);
            q = q1;
            Console.WriteLine("q = " + q);

            N = BigInteger.Multiply(p, q);
            Console.WriteLine("n = " + N);

            F = BigInteger.Multiply(BigInteger.Add(q, -1), BigInteger.Add(p, -1));
            Console.WriteLine("F = " + F);

            d = Euclid(e1, F);
            Console.WriteLine("d = " + d);
            Console.WriteLine("Проверка");
            BigInteger prov = BigInteger.Multiply(d, e1);
            Console.WriteLine("{0} * {1} = {2}, " + '\n' + "  {2} mod {3} = {4}", d, e1, prov, F, BigInteger.Remainder(prov, F));

        }


        public static BigInteger Encr(BigInteger X,BigInteger e)
        {
            return BinPowModul(X,e,N);
        }

        public static BigInteger Decr(BigInteger Y)
        {
            return BinPowModul(Y, d, N);
        }

        public static BigInteger DecrOptm(BigInteger Y)
        {
            BigInteger Q = Euclid(q, p);
            BigInteger x1 = BinPowModul(Y, d % (p - 1), p);
            BigInteger x2 = BinPowModul(Y, d % (q - 1), q);
            BigInteger x = (x2 + q * (Q * (x1 - x2) % p)) % N;
            if (x < 0) return x + N;
            else return x;
        }

        public static void Test()
        {
            string s = "659595556654433";
            BigInteger rand = RandomIntegerBelow(BigInteger.Parse(s));
            Console.WriteLine("Рандомное число < {1}  =  {0}", rand, s);
            string pow = "45599849989";
           
            Console.WriteLine("Бинарное представление {1}: {0} \n", ToBinaryString(BigInteger.Parse(s)), s);
            Console.WriteLine("2 ^ 2055 = {0} \n", BinPow(2, 2055));
            Console.WriteLine("2 ^ 2055 = {0} (mod 507) \n", BinPowModul(2, 2055, 507));
            Console.WriteLine("NOD({0},{1}) = {2} \n", s, pow, NOD(BigInteger.Parse(s), BigInteger.Parse(pow)));
            Console.WriteLine("Тест Ферма (1) 139 - {0} \n", IsPrime(139));
            int len = 13;
            Console.WriteLine("Генерация простого числа длины {0} = {1} \n", len, GenPrimes(len));
            BigInteger a1 = BigInteger.Parse("54356749384737828288");
            BigInteger m = BigInteger.Parse("8987676749384737828288");
            BigInteger a2 = Euclid(a1, m);
            if (a2 == -1)
                Console.WriteLine("Обратное число по модулю {0} ^ (-1) mod {1} = {2} \n", a1, m, "Нельзя!");
            else Console.WriteLine("Обратное число по модулю {0} ^ (-1) mod {1} = {2} \n", a1, m, a2);


            a1 = BigInteger.Parse("54356749384737828281");
            m = BigInteger.Parse("8987676749384737828288");
            a2 = Euclid(a1, m);
            Console.WriteLine("Обратное число по модулю {0} ^ (-1) mod {1} = {2}", a1, m, a2);
            Console.WriteLine("Проверка");
            BigInteger prov = BigInteger.Multiply(a2, a1);
            Console.WriteLine("{0} * {1} = {2}, " + '\n' + "  {2} mod {3} = {4} \n", a1, a2, prov, m, BigInteger.Remainder(prov, m));
           
        }

        public static void Test1()
        {
            Console.WriteLine("Начало сеанса RSA.........(ТЕСТ) \n ");

            GenTest(10, 7, 13, 11);
            BigInteger X = 7;
            Console.WriteLine("Сообщение: " +  X);

            BigInteger Y = Encr(X,7);
            Console.WriteLine("Зашифрованное сообщение: " + Y);
            Console.WriteLine("Расшифрованное сообщение: " + Decr(Y));
            Console.WriteLine("Расшифрованное сообщение (optm): " + DecrOptm(Y) + '\n' + '\n');
        }


        static void Main(string[] args)
        {
           // Test();
           // Test1();

            Console.WriteLine("Начало сеанса RSA......... \n ");
     
            Gen(l);
            BigInteger X;
            do
            {
                Console.WriteLine("Число: ");
                string plainText = Console.ReadLine();
                try
                {
                    X = BigInteger.Parse(plainText);
                }
                catch (Exception ex)
                { 
                    Console.WriteLine("Ошибка!");
                    X = 0;
                };
               
            } while (X.CompareTo(0) == 0 || X.CompareTo(N) == 1 || X.CompareTo(0) == -1 || X.CompareTo(N) == 0);

            BigInteger Y = Encr(X,e);
            Console.WriteLine("Зашифрованное сообщение: " + Y);
            Console.WriteLine("Расшифрованное сообщение: " + Decr(Y));
            Console.WriteLine("Расшифрованное сообщение (optm): " + DecrOptm(Y));
            Console.ReadKey();
            
        }
    }
}
