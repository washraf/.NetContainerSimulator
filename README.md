## .Net Container Simulator
.net Container Simulator is the first Container and Cloud Simulation tool built in .Net Framework.
The Simulator allows the implementation of multiple policies, Currently the Implemented Policies are:

* Elasticity Management: Under Publication
* Hanafy et al.- 10.1109/ICCES.2017.8275296
* Forsman et al. - 10.1016/j.jss.2014.11.044
* Zhao and Huang - 10.1109/NCM.2009.350

#### Scenarios
The simulator is equibed with multiple parameters for different simulations such as:
* Size
* Start Percent
* Mid-time Action
* Managenet Strategy
* Auction Type
* Initial Scheduling Algorithm
* Testing Threshould
#### Modeling Container Migration :
The migration time is measured using runC benchmarking tool [“runcbm”][3] developed by the research team.  

![alt text][im1]

#### Infrastructure Architecture 

![alt text][im2]

#### CaaS Architecture Model
![alt text][im3]

#### Simulator Architecture
![alt text][im4]

#### Simulation Design
CaaS datacenter Simulation encompasses different elements such as machines, containers, and switches. In addition, the simulation must include the simulation of different policies for management purposes. The main classes of our simulation are depicted in Figure ‎3.5.  
![alt text][im5]

#### Performance Evaluation Metrics
The Simular Accounting Module is able to measure:
* RMSE (btween Actual and Needed Hosts)
* Load Balancing (Entropy, Standard Deviation)
* Power Consumption
* Migrations Count 
* SLA
* Downtime
* Transmitted Data
* Number of Requests
* Hosts states
* Number of Running Hosts
* Image Pulls statistics 
* Containers
* Containers Per Host

[1]: https://runc.io/ 
[2]: https://www.criu.org/Main_Page
[3]: https://github.com/washraf/runcbm
[im1]: https://github.com/washraf/.NetContainerSimulator/blob/master/Images/CRtimes.jpg
[im2]: https://github.com/washraf/.NetContainerSimulator/blob/master/Images/Infrastructure.jpg
[im3]: https://github.com/washraf/.NetContainerSimulator/blob/master/Images/Architecture.jpg
[im4]: https://github.com/washraf/.NetContainerSimulator/blob/master/Images/SimulatorArchitecture.jpg
[im5]: https://github.com/washraf/.NetContainerSimulator/blob/master/Images/SimulatorDesign.jpg