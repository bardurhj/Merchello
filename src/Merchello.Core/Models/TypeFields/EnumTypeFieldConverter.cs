﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Core.Models.TypeFields
{

    public class EnumTypeFieldConverter
    {

        /// <summary>
        /// Creates an instance of an <see cref="IAddressTypeField"/> object
        /// </summary>
        internal static IAddressTypeField Address()
        {
            return new AddressTypeField();
        }

        /// <summary>
        /// Creates an instance of an <see cref="ICustomerRegistryTypeField"/> object
        /// </summary>
        internal static ICustomerRegistryTypeField CustomerRegistry()
        {
            return new CustomerRegistryTypeField();            
        }

        /// <summary>
        /// Creates an instance of an <see cref="IInvoiceItemTypeField"/> object
        /// </summary>
        /// <returns></returns>
        internal static IInvoiceItemTypeField InvoiceItem()
        {
            return new InvoiceItemTypeField();
        }

        /// <summary>
        /// Creates an instance of an <see cref="IPaymentMethodTypeField"/> object
        /// </summary>
        /// <returns></returns>
        internal static IPaymentMethodTypeField PaymentMethod()
        {
            return new PaymentMethodTypeField();
        }

        /// <summary>
        /// Creates an instance of an <see cref="IShipmentMethodTypeField"/> object
        /// </summary>
        /// <returns></returns>
        internal static IShipmentMethodTypeField ShipmentMethod()
        {
            return new ShipMethodTypeField();
        }

        /// <summary>
        /// Creates an instance of an <see cref="IAppliedPaymentTypeField"/>
        /// </summary>
        /// <returns></returns>
        internal static IAppliedPaymentTypeField AppliedPayment()
        {
            return new AppliedPaymentTypeField();
        }

        /// <summary>
        /// Creates an instance of an <see cref="IGatewayProviderTypeField"/>
        /// </summary>
        /// <returns></returns>
        internal static IGatewayProviderTypeField GatewayProvider()
        {
            return new GatewayProviderTypeField();            
        }
    }
}
