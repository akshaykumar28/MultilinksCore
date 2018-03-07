﻿export class DeviceDetail {

   /* The properties in this class should those of Endpoint object in
    * the backend (i.e. device == endpoint).
    */
   endpointId: string;  /* String representation of GUID. */
   creatorId: string;   /* String representation of GUID. */
   name: string;
   description: string;
}