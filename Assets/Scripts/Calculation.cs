using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Debug = System.Diagnostics.Debug;
using System.Numerics;


public class Calculation : MonoBehaviour
{
    public float coefficient = 1/Mathf.Sqrt(3);
    public float R;
    public float X;
    private Complex Y;
    private float S;
    private Complex Zc;

    private Complex gama;
    private double alpha,beta;
    private Complex ePowPozitiveY,ePowNegativeY;
    public Complex A,B,C,D; 
    
    private Complex Z;
    private Complex Vspn1;
    private Complex ComplexI;
    private Complex Ispn1;
    private float Theta;
    private float I;
    private float vrpn;
    private float length;
    private double Pvr;
    private double Ps1;
    private float pf1;
    private float pf2;
    private double Qs1;
    private float Ploss;
    private float Qloss;
    private float Pr1;
    public float SrCo;
    public bool isLong = false;
    
    
    [Header("Pf2 Params")] 
    private float Ir2Mag;
    private Complex Ir2;
    private float Theta2;
    private Complex Vspn2;
    private Complex Ispn2;
    private double Ps2;
    private double Qs2;
    private float Pvr2;
    private float Pr2;
    private float P2loss;
    private float Q2loss;

    
    [Header("Cable Inputs")]
    public InputField lengthText;
    public InputField RPerKmText;
    public InputField XPerKmText;
    public InputField SPerKmText;
    
    [Header("PF Inputs")]
    public InputField pf1Text;
    public InputField pf2Text;
    
    [Header("Reciveing Inputs")]
    public InputField Vrpn;
    public InputField Sr;

    [Header("Solution Texts")] 
    public Text AnDReal, AnDImg;
    public Text BReal, BImg;
    public Text CReal, CImg;
    
    public Text RText;
    public Text XText;
    
    public Text ReciveingCurrentWithAngle;
    public Text ReciveingCurrent;
    
    public Text SendingEndVoltage;
    public Text SendingEndVoltageAngle;
    
    public Text SendingEndPower;
    public Text SendingEndVoltageReactive;

    public Text Ispn1WithAngle;
    public Text Ispn1Current;

    public Text activePowerLostText;
    public Text reactivePowerLostText;
    
    public Text efficiencyText;
    public Text regulationText;

    [Header("PF2 Solution Texts")] 
    public Text PF2ReciveingCurrentWithAngle;
    public Text PF2ReciveingCurrent;

    public Text SendingEndVoltage2;
    public Text SendingEndVoltage2Angle;
    
    public Text Ispn2WithAngle;
    public Text Ispn2Current;

    public Text SendingEndPower2Text;
    public Text SendingEndVoltageReactive2Text;

    public Text regulationTextPF2;

    public Text efficiency2Text;
    public Text activePowerLost2Text;
    public Text reactivePowerLost2Text;

    [Header("Swtıcher Params")] 
    public int buttonStatus = 0;
    public Text calcuterText;
    public Text islongButtonText;
    public GameObject sPerKmText;
    public GameObject sPerKminput;

    
    private bool megaCo;

    public void MediumtoLongButton()
    {
        Color textColor = Color.white;
        buttonStatus++;
        if (buttonStatus == 3)
        {
            
            buttonStatus %= 3;
        }
        
        //Switch colculation button
        switch (buttonStatus)
        {
            case 2:
                calcuterText.text = "LONG LENGTH LINE CALCULATOR";
                islongButtonText.text =  "For Short";
                sPerKminput.SetActive(true);
                sPerKmText.GetComponent<Text>().color = textColor;
                break;
            case 1:
                calcuterText.text = "MEDIUM LENGTH LINE CALCULATOR";
                islongButtonText.text =  "For Long";
                sPerKminput.SetActive(true);
                sPerKmText.GetComponent<Text>().color = textColor;
                break;
            default:
                calcuterText.text = "SHORT LENGTH LINE CALCULATOR";
                islongButtonText.text =  "For Medium";
                sPerKminput.SetActive(false);
                textColor = Color.gray;
                textColor.a = 0.3f;
                sPerKmText.GetComponent<Text>().color = textColor;
                break;
        }
        
        UnityEngine.Debug.Log(buttonStatus);
    }
    
    
    public void CalculationFunction()
    {
        
        vrpn = float.Parse(Vrpn.text) * coefficient;
        length = float.Parse(lengthText.text) * 1;
        pf1 = float.Parse(pf1Text.text) / 100;
        pf2 = float.Parse(pf2Text.text) / 100;
        

        EmpedansFinder();
        admittanceFinder();

        switch (buttonStatus)
        {
            //For Long
            case 2:
                AbcdFinder();
                break;
            //For Medium
            case 1:
                AbcdFinderForMedium();
                break;
            //For Short
            default:
                abcdFinderForShorth();
                break;
        }
        
        currentFinder();
        sendingEndVoltageFinder();
        sendingEndCurrentFinder();
        sendingEndPowerFinder();
        voltageRegulatorFinder();
        activePowerLineLossesFinder();
        reactivePowerLineLossesFinder();
        transmissionLineEfficiencyFinder();
        
        //Pf2 Sols
        
        ir2Finder();
        sendingEndVoltage2Finder();
        sendingEndCurrent2Finder();
        sendingEndPower2Finder();
        activePowerLineLosses2Finder();
        reactivePowerLineLosses2Finder();
        voltageRegulatorPF2Finder();
        transmissionLineEfficiency2Finder();



    }
    

    void AbcdFinder()
    {
        gama = Complex.Sqrt(Complex.Multiply(Z , Y));
        UnityEngine.Debug.Log("gama: " + gama.ToString());
        alpha = Math.Round(gama.Real, 10);
        beta = Math.Round(gama.Imaginary, 10);
        
       // UnityEngine.Debug.Log("alpha: " + alpha.ToString());
        //UnityEngine.Debug.Log("beta: " + beta.ToString());
        
        ePowPozitiveY = Complex.FromPolarCoordinates(Math.Pow(Math.E, alpha), beta);
        ePowNegativeY = Complex.FromPolarCoordinates(Math.Pow(Math.E, -alpha), -beta);

        UnityEngine.Debug.Log("1/Zc: " + (1 / Zc).ToString());

        A = D = (ePowPozitiveY + ePowNegativeY) / 2;
        B = Complex.Multiply(Zc, ((ePowPozitiveY - ePowNegativeY) / 2));
        C = Complex.Multiply(1/Zc, ((ePowPozitiveY - ePowNegativeY) / 2));
        
        UnityEngine.Debug.Log("A = D: " + A.Phase.ToString());
        
        //UnityEngine.Debug.Log("((ePowPozitiveY - ePowNegativeY) / 2: " + ((ePowPozitiveY - ePowNegativeY) / 2).ToString());
        
        UnityEngine.Debug.Log("A=D: " + A.ToString());
        AnDReal.text = A.Real.ToString() + " , ";
        AnDImg.text = A.Imaginary.ToString() + "j";
        
        UnityEngine.Debug.Log("B: " + B.ToString());
        BReal.text = B.Real.ToString() + " , ";
        BImg.text = B.Imaginary.ToString() + "j";
        
        UnityEngine.Debug.Log("C: " + C.ToString());
        CReal.text = C.Real.ToString() + " , ";
        CImg.text = C.Imaginary.ToString() + "j";
        
    }
    
    void AbcdFinderForMedium()
    {
        gama = Complex.Sqrt(Complex.Multiply(Z , Y));
        UnityEngine.Debug.Log("gama: " + gama.ToString());
        alpha = Math.Round(gama.Real, 10);
        beta = Math.Round(gama.Imaginary, 10);
        
        UnityEngine.Debug.Log("alpha: " + alpha.ToString());
        UnityEngine.Debug.Log("beta: " + beta.ToString());
        
        ePowPozitiveY = Complex.FromPolarCoordinates(Math.Pow(Math.E, alpha), beta);
        ePowNegativeY = Complex.FromPolarCoordinates(Math.Pow(Math.E, -alpha), -beta);

        UnityEngine.Debug.Log("1/Zc: " + (1 / Zc).ToString());

        A = D = 1 + Complex.Multiply(Z, Y ) / 2;
        B = Z;
        C = Complex.Multiply(Y, (1 + Complex.Multiply(Z, Y) / 4));
        
        UnityEngine.Debug.Log("((ePowPozitiveY - ePowNegativeY) / 2: " + ((ePowPozitiveY - ePowNegativeY) / 2).ToString());
        
        UnityEngine.Debug.Log("A=D: " + A.ToString());
        AnDReal.text = A.Real.ToString() + " , ";
        AnDImg.text = A.Imaginary.ToString() + "j";
        
        UnityEngine.Debug.Log("B: " + B.ToString());
        BReal.text = B.Real.ToString() + " , ";
        BImg.text = B.Imaginary.ToString() + "j";
        
        UnityEngine.Debug.Log("C: " + C.ToString());
        CReal.text = C.Real.ToString() + " , ";
        CImg.text = C.Imaginary.ToString() + "j";
        
    }
    
    void abcdFinderForShorth()
    {
        A = D = 1;
        B = Z;
        C = 0;
        
        UnityEngine.Debug.Log("A=D: " + A.ToString());
        AnDReal.text = A.Real.ToString() + " , ";
        AnDImg.text = A.Imaginary.ToString() + "j";
        
        UnityEngine.Debug.Log("B: " + B.ToString());
        BReal.text = B.Real.ToString() + " , ";
        BImg.text = B.Imaginary.ToString() + "j";
        
        UnityEngine.Debug.Log("C: " + C.ToString());
        CReal.text = C.Real.ToString() + " , ";
        CImg.text = C.Imaginary.ToString() + "j";
    }
    
    void EmpedansFinder()
    {
            
        float RPerKm = float.Parse(RPerKmText.text);
        float XPerKm = float.Parse(XPerKmText.text);

        R = length * RPerKm;
        X = length * XPerKm;
        Z = new Complex(R, X);
        Zc = Complex.Sqrt(Z / Y);
        UnityEngine.Debug.Log("Zc: " + Zc.ToString());

        //string Rstr = 
        RText.text = "R: " + R.ToString();
        XText.text = "X: " + X.ToString();
    }
    
    float DegreesToRadians(float degrees)
    {
        return (float) (degrees * Mathf.PI / 180.0);
    }

    //Receiving Current
    void currentFinder()
    {
        I = float.Parse(Sr.text) / (3 * vrpn);
        Theta = -Mathf.Acos(pf1) * 180 / Mathf.PI;
        //Ir1pn
        ComplexI = Complex.FromPolarCoordinates(I, DegreesToRadians(Theta));
        UnityEngine.Debug.Log(ComplexI);
        

        ReciveingCurrentWithAngle.text = "Ir1pn: " + I.ToString() + "Φ" + Theta.ToString();
        ReciveingCurrent.text = "="  + (I * Mathf.Cos(DegreesToRadians(Theta))).ToString() +
                                "  " + (I * Mathf.Sin(DegreesToRadians(Theta))).ToString() + "j";
    }

    void sendingEndVoltageFinder()
    {
        //Vs1pn=A*Vr1pn+B*Irpn
        Vspn1 =  Complex.Multiply(A,vrpn) + Complex.Multiply(ComplexI, B);
        SendingEndVoltage.text =" =" + Math.Round(Vspn1.Real, 3).ToString() + ", " +  Math.Round(Vspn1.Imaginary, 3).ToString() +"j ";
        SendingEndVoltageAngle.text = "Vs1pn: " + Math.Round(Complex.Abs(Vspn1), 3).ToString() + "Φ" + Math.Round(Vspn1.Phase * 180/Math.PI, 3).ToString();
    }

    void sendingEndCurrentFinder()
    {
        //Is1pn=C*Vrpn+D*Ir1pn
        Ispn1 = Complex.Multiply(C, vrpn) + Complex.Multiply(D, ComplexI);
        UnityEngine.Debug.Log("Ispn1: " + Ispn1.ToString());
        Ispn1WithAngle.text = "Is1pn: " + Math.Round(Complex.Abs(Ispn1), 4).ToString() + "Φ" + Math.Round( Ispn1.Phase * 180/Mathf.PI , 4).ToString();
        Ispn1Current.text = "="  + Math.Round(Ispn1.Real , 4).ToString() +
                            "  " + Math.Round(Ispn1.Imaginary, 4).ToString() + "j";
    }


    void sendingEndPowerFinder()
    {
        Ps1 = 3 * Complex.Abs(Vspn1) * Complex.Abs(Ispn1) * Mathf.Cos((float) Vspn1.Phase - (float) Ispn1.Phase);
        Qs1 = 3 * Complex.Abs(Vspn1) * Complex.Abs(Ispn1) * Mathf.Sin((float) Vspn1.Phase - (float) Ispn1.Phase);
        
        SendingEndPower.text = "Ps1: " +  string.Format( "{0:e}" ,Math.Round(Ps1, 3)).ToString();
        SendingEndVoltageReactive.text = "Qs1: " +  string.Format( "{0:e}" ,Math.Round(Qs1, 3)).ToString();
    }
    

    void voltageRegulatorFinder()
    {
        Pvr = 100 * ((Complex.Abs(Vspn1)/Complex.Abs(A) - vrpn)/vrpn) ;
        UnityEngine.Debug.Log("Vreg: %" + Math.Round(Pvr, 4));
        regulationText.text = "V Reg:" + Math.Round(Pvr, 2) + "%";
    }

    void activePowerLineLossesFinder()
    {
        Pr1 = (float) float.Parse(Sr.text) * pf1;
        Ploss = (float) Ps1 - Pr1;
        UnityEngine.Debug.Log("Ploss:" + Math.Round(Ploss, 3) +"W");
        activePowerLostText.text = "Ploss:" + string.Format( "{0:e}" ,Math.Round(Ploss, 3)) + "W";
    }


    void reactivePowerLineLossesFinder()
    {
        float phaseOfIr = (float) ComplexI.Phase;
        float Qr1 =(float) float.Parse(Sr.text) * Mathf.Sin(0 - phaseOfIr);
        Qloss = (float) Qs1-Qr1;
        UnityEngine.Debug.Log("Qloss:" + string.Format( "{0:e}" ,Math.Round(Ploss, 3)) +"KVA");
        reactivePowerLostText.text = "Qloss:" + string.Format( "{0:e}" ,Math.Round(Qloss, 3)) +"KVA";
    }

    void transmissionLineEfficiencyFinder()
    {
        float lineEfficiency = (Pr1 / (Pr1 + Ploss)) * 100;
        UnityEngine.Debug.Log("L Efficiency:" + Math.Round(lineEfficiency, 2)+"%");
        efficiencyText.text = "Line Efc:" + Math.Round(lineEfficiency, 2) + "%";

    }

    void admittanceFinder()
    {
        float SPerKm;
        if (buttonStatus == 0)
        {
            SPerKm = 0;
        }
        else
        {
            SPerKm = float.Parse(SPerKmText.text);
        }
        Y = new Complex(0, SPerKm*length);
        UnityEngine.Debug.Log("Y: " + Y.ToString());
    }
    

    void ir2Finder()
    {
        Ir2Mag = Pr1 / (3 * vrpn * pf2);
        
        Theta2 = -Mathf.Acos(pf2) * 180 / Mathf.PI;
        Ir2 = Complex.FromPolarCoordinates(Ir2Mag, DegreesToRadians(Theta2));
        
        PF2ReciveingCurrentWithAngle.text = "Ir2pn: " + Ir2Mag.ToString() + "Φ" + Theta2.ToString();
        PF2ReciveingCurrent.text = "="  + (Ir2Mag * Mathf.Cos(DegreesToRadians(Theta2))).ToString() +
                                "  " + (Ir2Mag * Mathf.Sin(DegreesToRadians(Theta2))).ToString() + "j";
    }

    void sendingEndVoltage2Finder()
    {
        UnityEngine.Debug.Log("Ir2 gardas: " + Ir2.ToString());
        Vspn2 =  Complex.Multiply(A,vrpn) + Complex.Multiply(Ir2, B);
        SendingEndVoltage2.text =" =" + Math.Round(Vspn2.Real, 3).ToString() + ", " +  Math.Round(Vspn2.Imaginary, 3).ToString() +"j ";
        SendingEndVoltage2Angle.text = "Vspn2: " + Math.Round(Complex.Abs(Vspn2), 3).ToString() + "Φ" + Math.Round(Vspn2.Phase * 180/Math.PI, 3).ToString();
    }
    
    void sendingEndCurrent2Finder()
    {
        //Is1pn=C*Vrpn+D*Ir1pn
        Ispn2 = Complex.Multiply(C, vrpn) + Complex.Multiply(D, Ir2);
        UnityEngine.Debug.Log("Ispn2: " + Ispn2.ToString());
        Ispn2WithAngle.text = "Ispn2: " + Math.Round(Complex.Abs(Ispn2), 4).ToString() + "Φ" + Math.Round( Ispn2.Phase * 180/Mathf.PI , 4).ToString();
        Ispn2Current.text = "="  + Math.Round(Ispn2.Real , 4).ToString() +
                            "  " + Math.Round(Ispn2.Imaginary, 4).ToString() + "j";
    }
    
    void sendingEndPower2Finder()
    {
        Ps2 = 3 * Complex.Abs(Vspn2) * Complex.Abs(Ispn2) * Mathf.Cos((float) Vspn2.Phase - (float) Ispn2.Phase);
        Qs2 = 3 * Complex.Abs(Vspn2) * Complex.Abs(Ispn2) * Mathf.Sin((float) Vspn2.Phase - (float) Ispn2.Phase);
        
        SendingEndPower2Text.text = "Ps2: " +  string.Format( "{0:e}" ,Math.Round(Ps2, 3)).ToString();
        SendingEndVoltageReactive2Text.text = "Qs2: " +  string.Format( "{0:e}" ,Math.Round(Qs2, 3)).ToString();
    }
    
    void voltageRegulatorPF2Finder()
    {
        Pvr2 = (float) (100 * ((Complex.Abs(Vspn2)/Complex.Abs(A) - vrpn)/vrpn));
        UnityEngine.Debug.Log("After Vreg:  %" + Math.Round(Pvr, 4));
        regulationTextPF2.text = "After VReg:" + Math.Round(Pvr2, 2) + "%";
    }
    
    void transmissionLineEfficiency2Finder()
    {
        float lineEfficiency2 = (Pr2 / (Pr2 + P2loss)) * 100;
        UnityEngine.Debug.Log("After Line Efficiency:" + Math.Round(lineEfficiency2, 2)+"%");
        efficiency2Text.text = "After Line Efc:" + Math.Round(lineEfficiency2, 2) + "%";
    }
    
    void activePowerLineLosses2Finder()
    {
        Pr2 = (float) float.Parse(Sr.text) * pf1;
        P2loss = (float) Ps2 - Pr2;
        UnityEngine.Debug.Log("P2loss:" + Math.Round(P2loss, 3) +"W");
        activePowerLost2Text.text = "P2loss:" + string.Format( "{0:e}" ,Math.Round(P2loss, 3)) + "W";
    }


    void reactivePowerLineLosses2Finder()
    {
        float phaseOfIr2 = (float) Ir2.Phase;
        float Qr2 = Pr2 * Mathf.Tan(0 - phaseOfIr2);
        Q2loss = (float) Qs2-Qr2;
        UnityEngine.Debug.Log("Q2loss:" + string.Format( "{0:e}" ,Math.Round(Q2loss, 3))+"KVA");
        reactivePowerLost2Text.text = "Q2loss:" + string.Format( "{0:e}" ,Math.Round(Q2loss, 3))+"KVA";
    }
}


