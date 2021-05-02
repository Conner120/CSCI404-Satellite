using System;

namespace OrbitalCalculator.Services
{
    public static class Stumpff
    {
        public static double c(double x,double k){
            switch (k)
            {
                case 0:
                    return c_0(x);
                case 1:
                    return c_1(x);
                default:
                    return c_k(x,k);
            }
        }
        public static double c_0(double x){
            if(x>0){
                return Math.Cos(Math.Sqrt(x));
            }else if(x<0){
                return Math.Cosh(Math.Sqrt(x));
            }else{
                return 0;
            }
        }
        public static double c_1(double x){
            if(x>0){
                double sqrt_x = Math.Sqrt(x);
                return Math.Sin(sqrt_x)/sqrt_x;
            }else if(x<0){
                double sqrt_x = Math.Sqrt(-1*x);
                return Math.Sinh(sqrt_x)/sqrt_x;
            }else{
                return 0;
            }
        }
        public static double c_k(double x,double k){
            return ((1/factorial(k))-c(x,k))/x;
        }
        public static double factorial(double k){
            double sum = k;
            for (double i = (k-1); i > 1; i++){
                sum+=i;
            }
            return sum+1;
        }
    }
}