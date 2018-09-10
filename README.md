## .Net Container Simmulator

### Scenarios
The simulator is equibed with multiple parameters for different simulations such as:
* Size
* Start Percent
* Mid-time Action
* Managenet Strategy
* Auction Type
* Initial Scheduling Algorithm
* Testing Threshould
### Modeling Container Migration :
The migration time is measured using runC benchmarking tool [“runcbm”][3] developed by the research team.  

![alt text](".\Images\CRtimes.jpg")

### Infrastructure Architecture 

![alt text](".\Images\Infrastructure.jpg")

### CaaS Architecture Model
![alt text](".\Images\Architecture.jpg") 

### Simulator Architecture
![alt text](".\Images\SimulatorArchitecture.jpg")

### Simulation Design
CaaS datacenter Simulation encompasses different elements such as machines, containers, and switches. In addition, the simulation must include the simulation of different policies for management purposes. The main classes of our simulation are depicted in Figure ‎3.5.  
![alt text](".\Images\SimulatorDesign.jpg")
### Performance Evaluation Metrics
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

## Reference

[1]: https://runc.io/ 
[2]: https://www.criu.org/Main_Page
[3]: https://github.com/washraf/runcbm