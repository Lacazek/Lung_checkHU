# CheckHU_lung
This script is used for Pulmonary stereotactic radiation therapy. It creates optimisation structures and calculate index for density override for PTV target according to PTVP strategy [1] for VMAT pulmonary stereotactic.
This code has been drawn up as part of an internship in Oncopole in Toulouse.
The code is written in French.

This code creates optimisation structure for VMAT only :
- Ring for gradient dose (3 rings)
- Lung - ITV
- Lung - PTV
- Lung - (PTV+1cm)
- Homolateral Lung - PTV
- PTV - ITV
- Medullary canal + 5 mm

The index is calculated in a ring named RingUH. This structure is created with a 15 mm ring around the ITV in the Average CT. 
The index is the ratio of voxel of HU less than -900 UH in RingUH under all the voxel in RingUH.
-900 HU was set because AcurosXB has trouble in density less than -930 HU. Nevertheless, threshold of -900 HU show good agreement with our data.
Further investigation will be necessary to refine it, if necessary.

This script ending with the possibility, for the user, to automatically set, for PTV structure, a value of -750 HU. Following our research, this value gives good agreement for PTV target coverage in Average CT
and GTV target coverage in every phase of CT scan with max value under 140% prescription dose. This method is simple, fast and reduces the risk of error.

![image](https://github.com/Lacazek/Lung_checkHU/assets/152266244/5aa600e5-46ce-4e50-bc63-bf3e2adb2967)



[1] (Wiant et al. On the validity of density overrides for VMAT lung SBRT planning. Medical Physics, 2014, vol. 41, no 8Part1, p. 081707)
