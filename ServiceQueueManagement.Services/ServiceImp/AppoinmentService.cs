﻿using ServiceQueueManagement.Core.DTOs;
using ServiceQueueManagement.Core.Models;
using ServiceQueueManagement.Core.Repositories;
using ServiceQueueManagement.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceQueueManagement.Services.ServiceImp
{
    public class AppoinmentService : IAppoinmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppoinmentService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        /// <summary>to trigger add apoinment method in apoinment service</summary>
        public void AddAppoinments()
        {
            var lstNotAssignedCustomers = _unitOfWork.CustomerService.GetAllNotAssignedCustomers();
            Appoinment appoinment = new Appoinment();

            foreach (var notAssignedCustomer in lstNotAssignedCustomers)
            {
                var lstEmployeeServiceByServiceID = _unitOfWork.employeeService.GetEmployeeServiceByServiceId(notAssignedCustomer.FkServiceId);

                List<Temp> temps = new List<Temp>();

                foreach (var employee in lstEmployeeServiceByServiceID)
                {
                    var lstAppoinments = _unitOfWork.appoinmentRepository.GetAppoinmentsByEmployeeIdOrCustomerId(employee.FkEmployeeId, notAssignedCustomer.FkCustomerId);
                    Temp tp = new Temp();
                    tp.FkEmployeeId = employee.FkEmployeeId;
                    tp.Count = lstAppoinments.Count;
                    tp.ServiceSlotId = lstAppoinments.Max(x => x.FkServiceSlotId) ?? 0;
                    temps.Add(tp);
                }

                var minQueue = temps.OrderBy(x => x.Count).FirstOrDefault();

                appoinment.FkCustomerServiceId = notAssignedCustomer.Id;
                appoinment.FkCustomerId = notAssignedCustomer.FkCustomerId;
                appoinment.FkEmployeeId = minQueue.FkEmployeeId;
                if (minQueue.ServiceSlotId != 0)
                {
                    if (minQueue.ServiceSlotId != 1)
                    {
                        for (int x = 1; x < minQueue.ServiceSlotId; x++)
                        {
                            if (!_unitOfWork.appoinmentRepository.IsExistAppoinmentByCustomerIdAndServiceSlotId(notAssignedCustomer.FkCustomerId, x))
                            {
                                minQueue.ServiceSlotId = x - 1;
                                break;
                            }
                        }
                    }

                }
                appoinment.FkServiceSlotId = minQueue.ServiceSlotId + 1;

                while(_unitOfWork.appoinmentRepository.IsExistAppoinmentByEmployeeAndServiceSlot(appoinment.FkEmployeeId, appoinment.FkServiceSlotId))
                {
                    appoinment.FkServiceSlotId = appoinment.FkServiceSlotId + 1;
                }

                _unitOfWork.appoinmentRepository.addAppoinment(appoinment);
                _unitOfWork.CustomerService.UpdateAssignedCustomerServices(appoinment.FkCustomerServiceId);
                _unitOfWork.CommitChanges();
            }

        }

        /// <summary>get the list of ongoing appoinments by given service slot</summary>
        /// <param name="serviceSlotId">integer value for identify service slot</param>
        /// <param name="customerId"></param>
        /// <returns>lsit of ongoing appoinments Dto object</returns>
        public List<OngoingAppoinmentsDto> GetOngoingAppoinmentsByServiceSlotIdOrCustomerId(int? serviceSlotId, int? customerId)
        {

            if (serviceSlotId == null && customerId == null)
            {
                return _unitOfWork.appoinmentRepository.GetOngoingAppoinmentsByServiceSlotId(serviceSlotId);
            }else if(serviceSlotId != null && customerId == null)
            {
                return _unitOfWork.appoinmentRepository.GetOngoingAppoinmentsByServiceSlotId(serviceSlotId);
            }else if (serviceSlotId == null && customerId != null)
            {
                return _unitOfWork.appoinmentRepository.GetOngoingAppoinmentsByCustomerId(customerId);
            }

            return null;
        }
    }

    internal class Temp
    {
        public int Count { get; set; }
        public int FkEmployeeId { get; set; }
        public int? ServiceSlotId { get; set; }

        public Temp()
        {
            ServiceSlotId = 0;
        }
    }
}
