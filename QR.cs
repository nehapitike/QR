using System;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;


namespace QR
{
    public class BLAS : Hub
    {
        public string[] Blas1(String mat1, String mat2, String connID)
        {
            string strC = "";         
            var A = JsonConvert.DeserializeObject<Matrix>(mat1);  // see below for MatObject class
            var B = JsonConvert.DeserializeObject<Matrix>(mat2);  // see below for MatObject class
            int N = A.size[1]; // the number of columns in matrix A 
            int M = A.size[0]; // number of rows in matrix A
            int k = B.size[0]; // number of rows in matrix B
            double[,] Q = new double[M, N]; // dot product D
            double[,] R = new double[k, k];
            double[] T = new double[M * N];
            string[] D = new string[N];
            double sum = 0;   
            // Solving for each row of R and each column of Q in a iteration.
            for (int i = 0; i < N; i++)
            {                
                for (int j = 0; j < M; j++)
                {
                    sum = sum + A.data[j, i] * A.data[j, i];
                }
                R[i, i] = Math.Sqrt(sum);            
                for (int j = 0; j < M; j++)
                {
                    Q[j, i] = A.data[j, i] / R[i, i];              
        
                }
                sum = 0;
                if (i < N-1)
                {
                    for (int d = i; d < N-1; d++)
                    {
                        sum = 0;
                        for (int j = 0; j < M; j++)
                        {
                            sum = sum + Q[j, i] * A.data[j, d + 1];
                        }
                        R[i, d + 1] = sum;
                        for (int j = 0; j < M; j++)
                        {
                            A.data[j, d + 1] = A.data[j, d + 1] - (Q[j, i] * R[i, d + 1]);
                        }
                    }                       
                }
                sum = 0;
               
            }

            double[,] y = new double[N, 1];
            // solving Q' * b into y
            for (int i =0; i < N; i++)
            {
                sum = 0;
                for (int j = 0; j<M;j++)
                {
                    sum = sum + Q[j, i] * B.data[j, 0];
                }
                y[i, 0] = sum;
              
            }
        
            double[,] x = new double[N,1];
            //Solving for Rx = y for x
            for (int i =0; i < N; i++)
            {
                x[i, 0] = 0;
            }
            x[N - 1, 0] = y[N - 1, 0] / R[N - 1, N - 1];
            for (int i = N-2; i >=0; i--)
            {
                sum = 0;
                for (int z = N - 1; z > i; z--)
                {
                    sum = sum + R[i, z] * x[z, 0];
                    x[i, 0] = (y[i, 0] - sum) / R[i, i];
                }
            }

            for (int i = 0; i < N; i++)
            {                               
                 strC = x[i, 0].ToString();                                
                 Clients.Client(connID).store(strC);                
            
            }
            D[1] = M.ToString();
            Clients.Client(connID).displayOutput();

            return D;
        }
        // This class can be generated using http://json2csharp.com/
        // Enter {"matrixType":"DenseMatrix", "data": [[1,2], [3,4], [5,6]], "size": [3, 2]}
        public class Matrix
        {
            public string matrixType { get; set; }
            public double[,] data { get; set; }
            public int[] size { get; set; }
        }
    }
}
