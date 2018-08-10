using System;
using P01_HospitalDatabase.Data;
using P01_HospitalDatabase.Initializer;

namespace P01_HospitalDatabase
{
    class StartUp
    {
        static void Main(string[] args)
        {
            using(HospitalContext context = new HospitalContext())
            {
                DatabaseInitializer.InitialSeed(context);
            }
        }
    }
}
