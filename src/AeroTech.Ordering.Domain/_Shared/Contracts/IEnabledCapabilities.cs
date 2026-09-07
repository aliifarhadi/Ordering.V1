using AeroTech.Ordering.Domain._Shared.Attributes;
using AeroTech.Ordering.Domain._Shared.Enums;

namespace AeroTech.Ordering.Domain._Shared.Contracts
{
    [SpecRef("DD-CAPABILITIES-001")]
    [Blocked("BL-007", "Which item types, forms of payment, and channels are live", "ADR-16")]
    public interface IEnabledCapabilities
    {
        [SpecRef("DD-CAPABILITIES-001")]
        bool IsItemTypeEnabled(ItemType itemType);

        [SpecRef("DD-CAPABILITIES-001")]
        bool IsFormOfPaymentEnabled(FormOfPayment formOfPayment, ChannelCode channel);

        [SpecRef("DD-CAPABILITIES-001")]
        bool IsChannelEnabled(ChannelCode channel);
    }
}
