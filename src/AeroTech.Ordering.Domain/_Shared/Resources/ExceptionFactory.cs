using AeroTech.Framework.Core.Domain.Exceptions;

namespace AeroTech.Ordering.Domain._Shared.Resources
{
    public static class ExceptionFactory
    {
        // Structural invariants I-01..I-08: 2001-2029
        public static BusinessException OrderRequiresTravelerAndItem() =>
            new(2001, ExceptionMessages.OrderRequiresTravelerAndItem) { HttpStatus = 422 };

        public static BusinessException ItemRequiresExistingTraveler(params object?[] args) =>
            new(2002, ExceptionMessages.ItemRequiresExistingTraveler, args) { HttpStatus = 422 };

        public static BusinessException FlightItemRequiresJourneyReference() =>
            new(2003, ExceptionMessages.FlightItemRequiresJourneyReference) { HttpStatus = 422 };

        public static BusinessException AncillaryRequiresSingleJourneyReference() =>
            new(2004, ExceptionMessages.AncillaryRequiresSingleJourneyReference) { HttpStatus = 422 };

        public static BusinessException AncillaryRequiresConfirmedFlightItem(params object?[] args) =>
            new(2005, ExceptionMessages.AncillaryRequiresConfirmedFlightItem, args) { HttpStatus = 409 };

        public static BusinessException InfantRequiresAssociatedAdult(params object?[] args) =>
            new(2006, ExceptionMessages.InfantRequiresAssociatedAdult, args) { HttpStatus = 422 };

        public static BusinessException SegmentRequiresBothRefsPresent(params object?[] args) =>
            new(2007, ExceptionMessages.SegmentRequiresBothRefsPresent, args) { HttpStatus = 422 };

        public static BusinessException SegmentAlreadyExists(params object?[] args) =>
            new(2008, ExceptionMessages.SegmentAlreadyExists, args) { HttpStatus = 409 };

        public static BusinessException FlightItemRequiresSegmentPerJourney(params object?[] args) =>
            new(2009, ExceptionMessages.FlightItemRequiresSegmentPerJourney, args) { HttpStatus = 422 };

        public static BusinessException SaleCurrencyImmutableAfterPayment() =>
            new(2010, ExceptionMessages.SaleCurrencyImmutableAfterPayment) { HttpStatus = 409 };

        // Commercial invariants I-09..I-15: 2030-2059
        public static BusinessException PriceTotalMustEqualSumOfLines() =>
            new(2030, ExceptionMessages.PriceTotalMustEqualSumOfLines) { HttpStatus = 422 };

        public static BusinessException PriceLinesMustShareCurrency() =>
            new(2031, ExceptionMessages.PriceLinesMustShareCurrency) { HttpStatus = 422 };

        public static BusinessException PriceRequiresAtLeastOneLine() =>
            new(2032, ExceptionMessages.PriceRequiresAtLeastOneLine) { HttpStatus = 422 };

        public static BusinessException NegativeAmountNotPermittedForChargeType(params object?[] args) =>
            new(2033, ExceptionMessages.NegativeAmountNotPermittedForChargeType, args) { HttpStatus = 422 };

        public static BusinessException AmountDoesNotConformToCurrencyScale(params object?[] args) =>
            new(2034, ExceptionMessages.AmountDoesNotConformToCurrencyScale, args) { HttpStatus = 422 };

        public static BusinessException AmountDoesNotConformToCurrencyIncrement(params object?[] args) =>
            new(2035, ExceptionMessages.AmountDoesNotConformToCurrencyIncrement, args) { HttpStatus = 422 };

        public static BusinessException CurrencyMismatch(params object?[] args) =>
            new(2036, ExceptionMessages.CurrencyMismatch, args) { HttpStatus = 422 };

        public static BusinessException AllocationsCannotExceedAmount(params object?[] args) =>
            new(2037, ExceptionMessages.AllocationsCannotExceedAmount, args) { HttpStatus = 422 };

        public static BusinessException DeliveryRequiresSettledOrCredit(params object?[] args) =>
            new(2038, ExceptionMessages.DeliveryRequiresSettledOrCredit, args) { HttpStatus = 409 };

        public static BusinessException MultiSegmentRequiresValueAllocation() =>
            new(2039, ExceptionMessages.MultiSegmentRequiresValueAllocation) { HttpStatus = 422 };

        public static BusinessException ValueAllocationMustMatchItemTotal() =>
            new(2040, ExceptionMessages.ValueAllocationMustMatchItemTotal) { HttpStatus = 422 };

        // Lifecycle invariants I-16..I-22: 2060-2089
        public static BusinessException CannotCancelFlownItem(params object?[] args) =>
            new(2060, ExceptionMessages.CannotCancelFlownItem, args) { HttpStatus = 409 };

        public static BusinessException CannotRemoveTravelerWithActiveItem(params object?[] args) =>
            new(2061, ExceptionMessages.CannotRemoveTravelerWithActiveItem, args) { HttpStatus = 409 };

        public static BusinessException CannotRemoveJourneyElementWithActiveSegment(params object?[] args) =>
            new(2062, ExceptionMessages.CannotRemoveJourneyElementWithActiveSegment, args) { HttpStatus = 409 };

        public static BusinessException ExpiryOnlyAppliesToPendingPayment() =>
            new(2063, ExceptionMessages.ExpiryOnlyAppliesToPendingPayment) { HttpStatus = 409 };

        public static BusinessException ItemCannotTransition(params object?[] args) =>
            new(2064, ExceptionMessages.ItemCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException SegmentCannotTransition(params object?[] args) =>
            new(2065, ExceptionMessages.SegmentCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException DeliveryStatusCannotTransition(params object?[] args) =>
            new(2066, ExceptionMessages.DeliveryStatusCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException DeliveredCancellationRequiresVoidedUnits(params object?[] args) =>
            new(2067, ExceptionMessages.DeliveredCancellationRequiresVoidedUnits, args) { HttpStatus = 409 };

        // Governance invariants I-23..I-27: 2090-2109
        public static BusinessException ServicingRequiresMatchingSellerOrOverride() =>
            new(2090, ExceptionMessages.ServicingRequiresMatchingSellerOrOverride) { HttpStatus = 403 };

        public static BusinessException RejectedWhileUnderExternalControl(params object?[] args) =>
            new(2091, ExceptionMessages.RejectedWhileUnderExternalControl, args) { HttpStatus = 409 };

        public static BusinessException OrderIsSuspended() =>
            new(2092, ExceptionMessages.OrderIsSuspended) { HttpStatus = 409 };

        public static BusinessException OrderIsUnderLegalHold() =>
            new(2093, ExceptionMessages.OrderIsUnderLegalHold) { HttpStatus = 409 };

        public static BusinessException OrderIsClosed() =>
            new(2094, ExceptionMessages.OrderIsClosed) { HttpStatus = 409 };

        public static BusinessException PaymentIsUnderDispute(params object?[] args) =>
            new(2095, ExceptionMessages.PaymentIsUnderDispute, args) { HttpStatus = 409 };

        // Time limits: 2110-2129
        public static BusinessException TimeLimitNotFound(params object?[] args) =>
            new(2110, ExceptionMessages.TimeLimitNotFound, args) { HttpStatus = 404 };

        public static BusinessException TimeLimitNotActive(params object?[] args) =>
            new(2111, ExceptionMessages.TimeLimitNotActive, args) { HttpStatus = 409 };

        public static BusinessException TimeLimitExtensionsExhausted(params object?[] args) =>
            new(2112, ExceptionMessages.TimeLimitExtensionsExhausted, args) { HttpStatus = 409 };

        public static BusinessException TimeLimitMustBeInTheFuture() =>
            new(2113, ExceptionMessages.TimeLimitMustBeInTheFuture) { HttpStatus = 422 };

        public static BusinessException TimeLimitExtensionMustBeLater() =>
            new(2114, ExceptionMessages.TimeLimitExtensionMustBeLater) { HttpStatus = 422 };

        public static BusinessException ExpiryBlockedByOpenPaymentCompletion(params object?[] args) =>
            new(2115, ExceptionMessages.ExpiryBlockedByOpenPaymentCompletion, args) { HttpStatus = 409 };

        // Lookups and composition: 2130-2159
        public static BusinessException TravelerNotFound(params object?[] args) =>
            new(2130, ExceptionMessages.TravelerNotFound, args) { HttpStatus = 404 };

        public static BusinessException JourneyElementNotFound(params object?[] args) =>
            new(2131, ExceptionMessages.JourneyElementNotFound, args) { HttpStatus = 404 };

        public static BusinessException OrderItemNotFound(params object?[] args) =>
            new(2132, ExceptionMessages.OrderItemNotFound, args) { HttpStatus = 404 };

        public static BusinessException PaymentRecordNotFound(params object?[] args) =>
            new(2133, ExceptionMessages.PaymentRecordNotFound, args) { HttpStatus = 404 };

        public static BusinessException SegmentNotFound(params object?[] args) =>
            new(2134, ExceptionMessages.SegmentNotFound, args) { HttpStatus = 404 };

        public static BusinessException DuplicateTravelerId(params object?[] args) =>
            new(2135, ExceptionMessages.DuplicateTravelerId, args) { HttpStatus = 409 };

        public static BusinessException DuplicateJourneyElementId(params object?[] args) =>
            new(2136, ExceptionMessages.DuplicateJourneyElementId, args) { HttpStatus = 409 };

        public static BusinessException DuplicateOrderItemId(params object?[] args) =>
            new(2137, ExceptionMessages.DuplicateOrderItemId, args) { HttpStatus = 409 };

        public static BusinessException DuplicatePaymentRecordId(params object?[] args) =>
            new(2138, ExceptionMessages.DuplicatePaymentRecordId, args) { HttpStatus = 409 };

        // Limits: 2160-2179
        public static BusinessException TravelerLimitExceeded(params object?[] args) =>
            new(2160, ExceptionMessages.TravelerLimitExceeded, args) { HttpStatus = 422 };

        public static BusinessException JourneyElementLimitExceeded(params object?[] args) =>
            new(2161, ExceptionMessages.JourneyElementLimitExceeded, args) { HttpStatus = 422 };

        public static BusinessException ItemLimitExceeded(params object?[] args) =>
            new(2162, ExceptionMessages.ItemLimitExceeded, args) { HttpStatus = 422 };

        public static BusinessException NameIsTooLong(params object?[] args) =>
            new(2163, ExceptionMessages.NameIsTooLong, args) { HttpStatus = 422 };

        // Capabilities: 2180-2189
        public static BusinessException ItemTypeNotEnabled(params object?[] args) =>
            new(2180, ExceptionMessages.ItemTypeNotEnabled, args) { HttpStatus = 422 };

        public static BusinessException FormOfPaymentNotEnabled(params object?[] args) =>
            new(2181, ExceptionMessages.FormOfPaymentNotEnabled, args) { HttpStatus = 422 };

        public static BusinessException ChannelNotEnabled(params object?[] args) =>
            new(2182, ExceptionMessages.ChannelNotEnabled, args) { HttpStatus = 422 };

        // Composition validation: 2190-2219
        public static BusinessException IdentifierIsRequired(params object?[] args) =>
            new(2190, ExceptionMessages.IdentifierIsRequired, args) { HttpStatus = 422 };

        public static BusinessException GivenNameIsRequired() =>
            new(2191, ExceptionMessages.GivenNameIsRequired) { HttpStatus = 422 };

        public static BusinessException SurnameIsRequired() =>
            new(2192, ExceptionMessages.SurnameIsRequired) { HttpStatus = 422 };

        public static BusinessException DateOfBirthRequiredForPassengerType(params object?[] args) =>
            new(2193, ExceptionMessages.DateOfBirthRequiredForPassengerType, args) { HttpStatus = 422 };

        public static BusinessException ContactValueIsRequired() =>
            new(2194, ExceptionMessages.ContactValueIsRequired) { HttpStatus = 422 };

        public static BusinessException OnlyOnePrimaryContactPerType(params object?[] args) =>
            new(2195, ExceptionMessages.OnlyOnePrimaryContactPerType, args) { HttpStatus = 422 };

        public static BusinessException CurrencyCodeIsInvalid(params object?[] args) =>
            new(2196, ExceptionMessages.CurrencyCodeIsInvalid, args) { HttpStatus = 422 };

        public static BusinessException CancellationRequiresReason() =>
            new(2197, ExceptionMessages.CancellationRequiresReason) { HttpStatus = 422 };

        public static BusinessException AssociatedAdultMustNotBeSelf(params object?[] args) =>
            new(2198, ExceptionMessages.AssociatedAdultMustNotBeSelf, args) { HttpStatus = 422 };

        public static BusinessException AssociatedAdultMustBeAdult(params object?[] args) =>
            new(2199, ExceptionMessages.AssociatedAdultMustBeAdult, args) { HttpStatus = 422 };

        // Groups I-28, I-29: 2240-2269
        public static BusinessException CannotReleaseMoreSeatsThanHeld(params object?[] args) =>
            new(2240, ExceptionMessages.CannotReleaseMoreSeatsThanHeld, args) { HttpStatus = 422 };

        public static BusinessException AllocatedSlotsCannotExceedHeldBlock(params object?[] args) =>
            new(2241, ExceptionMessages.AllocatedSlotsCannotExceedHeldBlock, args) { HttpStatus = 422 };

        public static BusinessException SeatBlockNotFound(params object?[] args) =>
            new(2242, ExceptionMessages.SeatBlockNotFound, args) { HttpStatus = 404 };

        public static BusinessException NameSlotNotFound(params object?[] args) =>
            new(2243, ExceptionMessages.NameSlotNotFound, args) { HttpStatus = 404 };

        public static BusinessException SlotIsNotUnallocated(params object?[] args) =>
            new(2244, ExceptionMessages.SlotIsNotUnallocated, args) { HttpStatus = 409 };

        public static BusinessException SlotIsNotAllocated(params object?[] args) =>
            new(2245, ExceptionMessages.SlotIsNotAllocated, args) { HttpStatus = 409 };

        public static BusinessException SlotCannotTransition(params object?[] args) =>
            new(2246, ExceptionMessages.SlotCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException BlocksMustBeConfirmedBeforeAllocation() =>
            new(2247, ExceptionMessages.BlocksMustBeConfirmedBeforeAllocation) { HttpStatus = 409 };

        public static BusinessException GroupIsSuspended() =>
            new(2248, ExceptionMessages.GroupIsSuspended) { HttpStatus = 409 };

        public static BusinessException GroupIsClosed() =>
            new(2249, ExceptionMessages.GroupIsClosed) { HttpStatus = 409 };

        public static BusinessException DuplicateBlockId(params object?[] args) =>
            new(2250, ExceptionMessages.DuplicateBlockId, args) { HttpStatus = 409 };

        public static BusinessException DuplicateSlotId(params object?[] args) =>
            new(2251, ExceptionMessages.DuplicateSlotId, args) { HttpStatus = 409 };

        public static BusinessException SeatsHeldMustBePositive() =>
            new(2252, ExceptionMessages.SeatsHeldMustBePositive) { HttpStatus = 422 };

        // Delivery I-30, I-33, I-34: 2270-2299
        public static BusinessException DeliveryUnitUnderExternalControl(params object?[] args) =>
            new(2270, ExceptionMessages.DeliveryUnitUnderExternalControl, args) { HttpStatus = 409 };

        public static BusinessException SegmentDeliveryCannotTransition(params object?[] args) =>
            new(2271, ExceptionMessages.SegmentDeliveryCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException ServiceDeliveryCannotTransition(params object?[] args) =>
            new(2272, ExceptionMessages.ServiceDeliveryCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException VoidWindowExpired(params object?[] args) =>
            new(2273, ExceptionMessages.VoidWindowExpired, args) { HttpStatus = 409 };

        public static BusinessException AllUnitsMustBeOpenToVoid(params object?[] args) =>
            new(2274, ExceptionMessages.AllUnitsMustBeOpenToVoid, args) { HttpStatus = 409 };

        public static BusinessException DeliveryRecordRequiresAtLeastOneUnit() =>
            new(2275, ExceptionMessages.DeliveryRecordRequiresAtLeastOneUnit) { HttpStatus = 422 };

        public static BusinessException SegmentUnitNotFound(params object?[] args) =>
            new(2276, ExceptionMessages.SegmentUnitNotFound, args) { HttpStatus = 404 };

        public static BusinessException ServiceUnitNotFound(params object?[] args) =>
            new(2277, ExceptionMessages.ServiceUnitNotFound, args) { HttpStatus = 404 };

        public static BusinessException UnitValuesMustMatchAllocation() =>
            new(2278, ExceptionMessages.UnitValuesMustMatchAllocation) { HttpStatus = 422 };

        public static BusinessException ControlAlreadyHeld(params object?[] args) =>
            new(2279, ExceptionMessages.ControlAlreadyHeld, args) { HttpStatus = 409 };

        public static BusinessException ControlNotHeld(params object?[] args) =>
            new(2280, ExceptionMessages.ControlNotHeld, args) { HttpStatus = 409 };

        public static BusinessException ValueReturnedBeforeUnitsWithdrawn(params object?[] args) =>
            new(2281, ExceptionMessages.ValueReturnedBeforeUnitsWithdrawn, args) { HttpStatus = 409 };

        // Tax documents I-31, I-32: 2300-2319
        public static BusinessException ReversalRequiresReversedDocument(params object?[] args) =>
            new(2300, ExceptionMessages.ReversalRequiresReversedDocument, args) { HttpStatus = 422 };

        public static BusinessException InvoiceCannotReferenceReversedDocument() =>
            new(2301, ExceptionMessages.InvoiceCannotReferenceReversedDocument) { HttpStatus = 422 };

        public static BusinessException TaxDocumentRequiresAtLeastOneLine() =>
            new(2302, ExceptionMessages.TaxDocumentRequiresAtLeastOneLine) { HttpStatus = 422 };

        public static BusinessException TaxDocumentTotalsMustMatchLines() =>
            new(2303, ExceptionMessages.TaxDocumentTotalsMustMatchLines) { HttpStatus = 422 };

        public static BusinessException SubmissionCannotTransition(params object?[] args) =>
            new(2304, ExceptionMessages.SubmissionCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException SpecVersionIsRequired() =>
            new(2305, ExceptionMessages.SpecVersionIsRequired) { HttpStatus = 422 };

        public static BusinessException PassengerIdentityIsRequired() =>
            new(2306, ExceptionMessages.PassengerIdentityIsRequired) { HttpStatus = 422 };

        // Document number ranges I-30: 2320-2339
        public static BusinessException RangeIsNotActive(params object?[] args) =>
            new(2320, ExceptionMessages.RangeIsNotActive, args) { HttpStatus = 409 };

        public static BusinessException RangeIsExhausted(params object?[] args) =>
            new(2321, ExceptionMessages.RangeIsExhausted, args) { HttpStatus = 409 };

        public static BusinessException RangeBoundsAreInvalid() =>
            new(2322, ExceptionMessages.RangeBoundsAreInvalid) { HttpStatus = 422 };

        public static BusinessException RangeKindMismatch(params object?[] args) =>
            new(2323, ExceptionMessages.RangeKindMismatch, args) { HttpStatus = 422 };

        // Operations: 2220-2239
        public static BusinessException OperationCannotTransition(params object?[] args) =>
            new(2220, ExceptionMessages.OperationCannotTransition, args) { HttpStatus = 409 };

        public static BusinessException OperationIsTerminal(params object?[] args) =>
            new(2221, ExceptionMessages.OperationIsTerminal, args) { HttpStatus = 409 };

        public static BusinessException OperationStepNotOpen(params object?[] args) =>
            new(2222, ExceptionMessages.OperationStepNotOpen, args) { HttpStatus = 409 };
    }
}
