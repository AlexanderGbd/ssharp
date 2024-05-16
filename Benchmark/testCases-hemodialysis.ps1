# The MIT License (MIT)
# 
# Copyright (c) 2014-2016, Institute for Software & Systems Engineering
# 
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
# 
# The above copyright notice and this permission notice shall be included in
# all copies or substantial portions of the Software.
# 
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
# THE SOFTWARE.


# Note: You must run the following command first
#  Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
# To Undo
#  Set-ExecutionPolicy -ExecutionPolicy Restricted -Scope CurrentUser

# It is easy to get the method names in an assembly by extracting them from TestResult.xml which is generated by nunit.exe


###############################################
# HemodialysisMachine
###############################################

AddTest -Testname "Hemodialysis_PermanentWaterHeaterFault_Probability_HazardUnsuccessful" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterPermanentDemandKeep" -TestNunitCategory "DialysisFinishedAndBloodNotCleaned" -TestCategories @("HemodialysisMachine","Hazard-Unsuccessful","Probability")
AddTest -Testname "Hemodialysis_PermanentWaterHeaterFault_Probability_HazardContamination" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterPermanentDemandKeep" -TestNunitCategory "IncomingBloodIsContaminated" -TestCategories @("HemodialysisMachine","Hazard-Contamination","Probability")
AddTest -Testname "Hemodialysis_PermanentWaterHeaterFault_FaultTree_HazardUnsuccessful" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterPermanentDemandKeep" -TestNunitCategory "DialysisFinishedAndBloodNotCleaned_FaultTree" -TestCategories @("HemodialysisMachine","Hazard-Unsuccessful","DCCA")
AddTest -Testname "Hemodialysis_PermanentWaterHeaterFault_FaultTree_HazardContamination" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterPermanentDemandKeep" -TestNunitCategory "IncomingBloodIsContaminated_FaultTree" -TestCategories @("HemodialysisMachine","Hazard-Contamination","DCCA")
AddTest -Testname "Hemodialysis_PermanentWaterHeaterFault_Probability_Parametric" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterPermanentDemandKeep" -TestNunitCategory "Parametric" -TestCategories @("HemodialysisMachine","Parametric")

AddTest -Testname "Hemodialysis_PermanentWaterHeaterFault_Probability_HazardUnsuccessful" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterPermanentDemandOnCustom" -TestNunitCategory "DialysisFinishedAndBloodNotCleaned" -TestCategories @("HemodialysisMachine","Hazard-Unsuccessful","Probability")
AddTest -Testname "Hemodialysis_PermanentWaterHeaterFault_Probability_HazardContamination" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterPermanentDemandOnCustom" -TestNunitCategory "IncomingBloodIsContaminated" -TestCategories @("HemodialysisMachine","Hazard-Contamination","Probability")
AddTest -Testname "Hemodialysis_PermanentWaterHeaterFault_FaultTree_HazardUnsuccessful" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterPermanentDemandOnCustom" -TestNunitCategory "DialysisFinishedAndBloodNotCleaned_FaultTree" -TestCategories @("HemodialysisMachine","Hazard-Unsuccessful","DCCA")
AddTest -Testname "Hemodialysis_PermanentWaterHeaterFault_FaultTree_HazardContamination" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterPermanentDemandOnCustom" -TestNunitCategory "IncomingBloodIsContaminated_FaultTree" -TestCategories @("HemodialysisMachine","Hazard-Contamination","DCCA")
AddTest -Testname "Hemodialysis_PermanentWaterHeaterFault_Probability_Parametric" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterPermanentDemandOnCustom" -TestNunitCategory "Parametric" -TestCategories @("HemodialysisMachine","Parametric")

AddTest -Testname "Hemodialysis_TransientWaterHeaterFault_Probability_HazardUnsuccessful" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterTransientDemandOnCustom" -TestNunitCategory "DialysisFinishedAndBloodNotCleaned" -TestCategories @("HemodialysisMachine","Hazard-Unsuccessful","Probability")
AddTest -Testname "Hemodialysis_TransientWaterHeaterFault_Probability_HazardContamination" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterTransientDemandOnCustom" -TestNunitCategory "IncomingBloodIsContaminated" -TestCategories @("HemodialysisMachine","Hazard-Contamination","Probability")
AddTest -Testname "Hemodialysis_TransientWaterHeaterFault_FaultTree_HazardUnsuccessful" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterTransientDemandOnCustom" -TestNunitCategory "DialysisFinishedAndBloodNotCleaned_FaultTree" -TestCategories @("HemodialysisMachine","Hazard-Unsuccessful","DCCA")
AddTest -Testname "Hemodialysis_TransientWaterHeaterFault_FaultTree_HazardContamination" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterTransientDemandOnCustom" -TestNunitCategory "IncomingBloodIsContaminated_FaultTree" -TestCategories @("HemodialysisMachine","Hazard-Contamination","DCCA")
AddTest -Testname "Hemodialysis_TransientWaterHeaterFault_Probability_Parametric" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterTransientDemandOnCustom" -TestNunitCategory "Parametric" -TestCategories @("HemodialysisMachine","Parametric")

AddTest -Testname "Hemodialysis_PermanentWaterHeaterFaultAllOnStartOfStep_Probability_HazardUnsuccessful" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterPermanentDemandOnStartOfStep" -TestNunitCategory "DialysisFinishedAndBloodNotCleaned" -TestCategories @("HemodialysisMachine","Hazard-Unsuccessful","Probability")
AddTest -Testname "Hemodialysis_PermanentWaterHeaterFaultAllOnStartOfStep_Probability_HazardContamination" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterPermanentDemandOnStartOfStep" -TestNunitCategory "IncomingBloodIsContaminated" -TestCategories @("HemodialysisMachine","Hazard-Contamination","Probability")
AddTest -Testname "Hemodialysis_PermanentWaterHeaterFaultAllOnStartOfStep_FaultTree_HazardUnsuccessful" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterPermanentDemandOnStartOfStep" -TestNunitCategory "DialysisFinishedAndBloodNotCleaned_FaultTree" -TestCategories @("HemodialysisMachine","Hazard-Unsuccessful","DCCA")
AddTest -Testname "Hemodialysis_PermanentWaterHeaterFaultAllOnStartOfStep_FaultTree_HazardContamination" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterPermanentDemandOnStartOfStep" -TestNunitCategory "IncomingBloodIsContaminated_FaultTree" -TestCategories @("HemodialysisMachine","Hazard-Contamination","DCCA")
AddTest -Testname "Hemodialysis_PermanentWaterHeaterFaultAllOnStartOfStep_Probability_Parametric" -TestAssembly "SafetySharp.CaseStudies.HemodialysisMachine.exe" -TestMethod "SafetySharp.CaseStudies.HemodialysisMachine.Analysis.HazardProbabilityTests.WaterHeaterPermanentDemandOnStartOfStep" -TestNunitCategory "Parametric" -TestCategories @("HemodialysisMachine","Parametric")
