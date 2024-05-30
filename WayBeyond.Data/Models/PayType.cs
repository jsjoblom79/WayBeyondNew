using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayBeyond.Data.Models
{
    public enum PayType
    {
        [Description("Medicare")]
        MEDICARE,
        [Description("Non-Medicare")]
        NON_MEDICARE,
        [Description("SelfPay")]
        SELF_PAY
    }
    public enum ClientId
    {
        LEA_TATUM_CLINIC_MC = 1306,
        LOVINGTON_STUDENT_CLINIC = 1296,
        NOR_LEA_BEHAV_HLTH_2ND = 1312,
        NOR_LEA_BEHAVIORAL_HLTH_MC = 1307,
        NOR_LEA_BEHAVIORAL_HEALTH = 1301,
        NOR_LEA_CANCER_CENTER = 1315,
        NOR_LEA_CANCER_CENTER_2ND = 1317,
        NOR_LEA_CANCER_CENTER_MC = 1316,
        NOR_LEA_FAMILY_HEALTH = 1302,
        NOR_LEA_FAMILY_HEALTH_2ND = 1303,
        NOR_LEA_FAMILY_HEALTH_MC = 1308,
        NOR_LEA_GENERAL_HOSPITAL = 1289,
        NOR_LEA_GENERAL_HOSPITAL2 = 1291,
        NOR_LEA_GENERAL_HOSPITAL_MC = 1290,
        NOR_LEA_HOBBS_MEDICAL = 1305,
        NOR_LEA_HOBBS_MEDICAL_2ND = 1314,
        NOR_LEA_HOBBS_MEICAL_MC = 1310,
        NOR_LEA_LOVINGTON_CLINIC = 1292,
        NOR_LEA_LOVINGTON_CL_MC = 1294,
        NOR_LEA_LOVINGTON_CLINIC_2 = 1293,
        NOR_LEA_PROF_PHYS_MC = 1309,
        NOR_LEA_PROFESS_HLTH_2ND = 1313,
        NOR_LEA_PROFESSIONAL_PHYS = 1304,
        NOR_LEA_TATUM_CLINIC = 1300,
        NOR_LEA_TATUM_CLINIC_2ND = 1311,
        UNIDENTIFIED = 9999
    }
}
