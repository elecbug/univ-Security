﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnivSecurity
{
    public class DES
    {
        public static DES? Service = null;
        
        public static void CreateDES()
        {
            if (DES.Service is null)
            {
                DES.Service = new DES();
            }
        }
        private DES()
        {

        }

        public void Encrypt()
        {
            BitArray left = new BitArray(32);
            BitArray right = new BitArray(32);

            for (int i = 0; i < 32; i++)
            {
                left[i] = this.input[i];
            }
            for (int i = 0; i < 32; i++)
            {
                right[i] = this.input[i + 32];
            }


        }

        private BitArray FunctionF(BitArray input, BitArray key)
        {
            if (input.Length != 32 || key.Length != 48)
            {
                throw new ArgumentException("Input array's length is not 32 or Key array's length is not 48");
            }

            BitArray expansion = new BitArray(48);

            for (int i = 0; i < 48; i++)
            {
                expansion[i] = input[Table.ExpansionPermutationTable[i]];
            }

            expansion = expansion.Xor(key);

            List<BitArray> blocks = new List<BitArray>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 6; j++) 
                {
                    blocks[i][j] = expansion[i * 8 + j];
                }
                blocks[i] = SBoxes(blocks[i], i);
            }
        }

        private BitArray SBoxes(BitArray input, int box_num)
        {
            if (input.Length != 6)
            {
                throw new ArgumentException("Input array's length is not 6");
            }

            int row 
                = (input[0] ? 1 : 0) * 0b10 + (input[5] ? 1 : 0) * 0b01;
            int column
                = (input[1] ? 1 : 0) * 0b1000 + (input[2] ? 1 : 0) * 0b0100 
                + (input[3] ? 1 : 0) * 0b0010 + (input[4] ? 1 : 0) * 0b0001;

            BitArray result = new BitArray(6);

            for (int i = 0; i < input.Length; i++)
            {
                result[i] = (Table.SBoxes[box_num][row, column] & (0b100_000 >> i)) != 0;
            }

            return result;
        }

        private BitArray input = new BitArray(64);
        public BitArray Input { get => this.input; }

        private BitArray output = new BitArray(64);
        public BitArray Output { get => this.output; }

        private BitArray key = new BitArray(56);
        public BitArray Key { get => this.key; }

        private class Table
        {
            public static readonly byte[] InitialPermutation =
            {
                58, 50, 42, 34, 26, 18, 10, 2,
                60, 52, 44, 36, 28, 20, 12, 4,
                62, 54, 46, 38, 30, 22, 14, 6,
                64, 56, 48, 40, 32, 24, 16, 8,
                57, 49, 41, 33, 25, 17, 9, 1,
                59, 51, 43, 35, 27, 19, 11, 3,
                61, 53, 45, 37, 29, 21, 13, 5,
                63, 55, 47, 39, 31, 23, 15, 7
                };

            public static readonly byte[] InverseInitialPermutation =
            {
                40, 8, 48, 16, 56, 24, 64, 32,
                39, 7, 47, 15, 55, 23, 63, 31,
                38, 6, 46, 14, 54, 22, 62, 30,
                37, 5, 45, 13, 53, 21, 61, 29,
                36, 4, 44, 12, 52, 20, 60, 28,
                35, 3, 43, 11, 51, 19, 59, 27,
                34, 2, 42, 10, 50, 18, 58, 26,
                33, 1, 41, 9, 49, 17, 57, 25
                };

            public static readonly byte[,] SBox1 = new byte[,]
            {
                {14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7},
                {0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8},
                {4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0},
                {15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13}
            };

            public static readonly byte[,] SBox2 = new byte[,]
            {
                {15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10},
                {3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5},
                {0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15},
                {13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9}
            };

            public static readonly byte[,] SBox3 = new byte[,]
            {
                {10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8},
                {13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1},
                {13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7},
                {1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12}
            };

            public static readonly byte[,] SBox4 = new byte[,]
            {
                {7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15},
                {13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9},
                {10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4},
                {3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14}
            };

            public static readonly byte[,] SBox5 = new byte[,]
            {
                {2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9},
                {14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6},
                {4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14},
                {11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3}
            };

            public static readonly byte[,] SBox6 = new byte[,]
            {
                {12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11},
                {10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8},
                {9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6},
                {4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13}
            };

            public static readonly byte[,] SBox7 = new byte[,]
            {
                {4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1},
                {13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6},
                {1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2},
                {6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12}
            };

            public static readonly byte[,] SBox8 = new byte[,]
            {
                {13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7},
                {1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2},
                {7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8},
                {2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11}
            };

            public static byte[][,] SBoxes = new byte[][,]
            {
                SBox1, SBox2, SBox3, SBox4,
                SBox5, SBox6, SBox7, SBox8,
            };

            public static readonly byte[] PermutationTable = new byte[]
            {
                15, 6, 19, 20, 28, 11, 27, 16,
                0, 14, 22, 25, 4, 17, 30, 9,
                1, 7, 23, 13, 31, 26, 2, 8,
                18, 12, 29, 5, 21, 10, 3, 24
            };

            public static readonly byte[] ExpansionPermutationTable = new byte[]
            {
                31, 0, 1, 2, 3, 4, 3, 4, 5, 6, 7, 8, 7, 8, 9, 10,
                11, 12, 11, 12, 13, 14, 15, 16, 15, 16, 17, 18, 19, 20, 19, 20,
                21, 22, 23, 24, 23, 24, 25, 26, 27, 28, 27, 28, 29, 30, 31, 0
            };
        }
    }
}