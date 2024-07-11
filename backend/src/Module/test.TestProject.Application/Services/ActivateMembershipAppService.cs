using Abp.Domain.Repositories;
using Abp.UI;
using Boxfusion.Membership.Common.Domain.Domain;
using Boxfusion.Membership.Common.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Shesha;
using Shesha.DynamicEntities.Dtos;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Boxfusion.Membership.Common.Services
{
    public class ActivateMembershipAppService : SheshaAppServiceBase
    {
        private readonly IRepository<Member, Guid> _memberRepo;
        private readonly IRepository<MembershipPayment, Guid> _membershipPaymentRepo;

        public ActivateMembershipAppService(IRepository<Member, Guid> memberRepo, IRepository<MembershipPayment, Guid> membershipPaymentRepo)
        {
            _memberRepo = memberRepo;
            _membershipPaymentRepo = membershipPaymentRepo;
        }

        [HttpPut, Route("[action]/{memberId}")]
        public async Task<DynamicDto<Member, Guid>> ActivateMembership(Guid memberId)
        {
            var member = await _memberRepo.GetAsync(memberId);
            var payments = await _membershipPaymentRepo.GetAllListAsync(data => data.Member.Id == memberId);

            if (payments.Count == 0) throw new UserFriendlyException("There no payments made");

            double totalAmount = 0;
            payments.ForEach(a =>
            {
                totalAmount += a.Amount;
            });

            if (totalAmount < 100) throw new UserFriendlyException("Payments made are less than 100");


            member.MembershipStatus = RefListMembershipStatuses.Active;
            var updatedMember = await _memberRepo.UpdateAsync(member);

            return await MapToDynamicDtoAsync<Member, Guid>(updatedMember);
        }

        [HttpPut, Route("[action]")]
        public async Task<IEnumerable<DynamicDto<Member, Guid>>> ActivateMemberships(List<Guid> memberIds)
        {
            var res = new List<DynamicDto<Member, Guid>>();
            var errorList = new List<string>();
            foreach (var memberId in memberIds) 
            {
                try
                {
                    var dtoMember = await ActivateMembership(memberId);

                    res.Add(dtoMember);
                }
                catch (Exception kk)
                {

                    errorList.Add(kk.Message);
                }
                
            }

            if (errorList.Count > 0)
            {
                throw new UserFriendlyException("Error Activating accounts " + string.Join(Environment.NewLine,errorList) );
            }

            return res;
        }
    }
}