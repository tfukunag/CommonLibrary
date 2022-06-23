using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary.FileIO;
using CommonLibrary.Measurement;
using CommonLibrary.Output;

namespace CommonLibrary.Basic
{
    public class parameterConverter
    {
        public static BasicComplex[,][] ToDifferentialParameter(BasicComplex[,][] data , bool symmetric)
        {
            if (data.GetLength(0) != 4) 
            {
                throw new Exception("入力データが4ポートSパラメータではありません。");
            }
            
            int points = data[0,0].Length;
            BasicComplex[,][] dummy  = new BasicComplex[4,4][];

            for (int i = 0; i < 4; i++) 
            {
                for (int j = 0; j < 4; j++) 
                {
                    dummy[j, i] = new BasicComplex[points];
                }
            }

            if (symmetric)
            {
                for (int i = 0; i < points; i++)
                {
                    dummy[0, 0][i] = data[0, 0][i] + data[2, 0][i];
                    dummy[1, 0][i] = data[0, 1][i] + data[2, 1][i];
                    dummy[2, 0][i] = new BasicComplex(0, 0);
                    dummy[3, 0][i] = new BasicComplex(0, 0);
                    dummy[0, 1][i] = data[1, 0][i] + data[3, 0][i];
                    dummy[1, 1][i] = data[1, 1][i] + data[3, 1][i];
                    dummy[2, 1][i] = new BasicComplex(0, 0);
                    dummy[3, 1][i] = new BasicComplex(0, 0);

                    dummy[0, 2][i] = new BasicComplex(0, 0);
                    dummy[1, 2][i] = new BasicComplex(0, 0);
                    dummy[2, 2][i] = data[0, 0][i] - data[2, 0][i];
                    dummy[3, 2][i] = data[0, 1][i] - data[2, 1][i];
                    dummy[0, 3][i] = new BasicComplex(0, 0);
                    dummy[1, 3][i] = new BasicComplex(0, 0);
                    dummy[2, 3][i] = data[1, 0][i] - data[3, 0][i];
                    dummy[3, 3][i] = data[1, 1][i] - data[3, 1][i];

                }
            }

            else 
            {
                for (int i = 0; i < points; i++)
                {
                    dummy[0, 0][i] = data[0, 0][i] + data[2, 0][i] + data[0, 2][i] + data[2, 2][i]/2;
                    dummy[1, 0][i] = data[0, 1][i] + data[2, 1][i] + data[0, 3][i] + data[2, 3][i] / 2;
                    dummy[2, 0][i] = data[0, 0][i] + data[2, 0][i] - data[0, 2][i] - data[2, 2][i] / 2;
                    dummy[3, 0][i] = data[0, 1][i] + data[2, 1][i] - data[0, 3][i] - data[2, 3][i] / 2;
                    dummy[0, 1][i] = data[1, 0][i] + data[3, 0][i] + data[1, 2][i] + data[3, 2][i] / 2;
                    dummy[1, 1][i] = data[1, 1][i] + data[3, 1][i] + data[1, 3][i] + data[3, 3][i] / 2;
                    dummy[2, 1][i] = data[1, 0][i] + data[3, 0][i] - data[1, 2][i] - data[3, 2][i] / 2;
                    dummy[3, 1][i] = data[1, 1][i] + data[3, 1][i] - data[1, 3][i] - data[3, 3][i] / 2;

                    dummy[0, 2][i] = data[0, 0][i] - data[2, 0][i] + data[0, 2][i] - data[2, 2][i] / 2;
                    dummy[1, 2][i] = data[0, 1][i] - data[2, 1][i] + data[0, 3][i] - data[2, 3][i] / 2;
                    dummy[2, 2][i] = data[0, 0][i] - data[2, 0][i] - data[0, 2][i] + data[2, 2][i] / 2;
                    dummy[3, 2][i] = data[0, 1][i] - data[2, 1][i] - data[0, 3][i] + data[2, 3][i] / 2;
                    dummy[0, 3][i] = data[1, 0][i] - data[3, 0][i] + data[1, 2][i] - data[3, 2][i] / 2;
                    dummy[1, 3][i] = data[1, 1][i] - data[3, 1][i] + data[1, 3][i] - data[3, 3][i] / 2;
                    dummy[2, 3][i] = data[1, 0][i] - data[3, 0][i] - data[1, 2][i] + data[3, 2][i] / 2;
                    dummy[3, 3][i] = data[1, 1][i] - data[3, 1][i] - data[1, 3][i] + data[3, 3][i] / 2; 

                }
            }
            return dummy;
        }

















//        private static TouchStoneFile ConverttoZ(TouchStoneFile ts)
//        {
//            ComplexMatrix[] parameterMatrix = new ComplexMatrix[ts.getParameter().GetLength(0)];
//            ComplexMatrix[] convertedParameter = new ComplexMatrix[ts.getParameter().GetLength(0)];
//            BasicComplex[,] dummy = new BasicComplex[ts.getParameter().GetLength(1), ts.getParameter().GetLength(2)];
//            BasicComplex[, ,] convertedData = new BasicComplex[ts.getParameter().GetLength(0), ts.getParameter().GetLength(1), ts.getParameter().GetLength(2)];
//            //下準備
//            for (int i = 0; i < ts.getParameter().GetLength(0); i++)
//            {
//                for (int j = 0; j < ts.getParameter().GetLength(1); j++)
//                {
//                    for (int k = 0; k < ts.getParameter().GetLength(2); k++)
//                    {
//                        dummy[j, k] = ts.getParameter()[i, j, k];
//                    }
//                }
//                parameterMatrix[i] = new ComplexMatrix(dummy);
//            }
//            //元データがZの場合：そのまま返す
//            if (ts.getParameterType()=="Z")
//            {
//                return ts;
//            }

//            //元データがY：逆行列にして返す
//            else if (ts.getParameterType()=="Y")
//            {
//                for (int i = 0; i < parameterMatrix.GetLength(0); i++)
//                {
//                    convertedParameter[i] = ComplexMatrix.inverseMatrix(parameterMatrix[i]);
//                }
//                for (int i = 0; i < ts.getParameter().GetLength(0); i++)
//                {
//                    for (int j = 0; j < ts.getParameter().GetLength(1); j++)
//                    {
//                        for (int k = 0; k < ts.getParameter().GetLength(2); k++)
//                        {
//                            convertedData[i, j, k] = convertedParameter[i].getMatrix()[j, k];
//                        }
//                    }
//                }
//                TouchStoneFile newts = new TouchStoneFile(ts.getFreq(),convertedData,ts.getZ0(),"Z");
                
//                return newts;
//            }

//            //元データがS：
//            else if (ts.getParameterType() == "S")
//            {
//                ComplexMatrix elementMat = ComplexMatrix.elementaryMatrix(ts.getParameter().GetLength(1));
 
                
//            }
//        }
    }
}
