/********************************************************************/
/*                                                                  */
/*       C/C++ Header File  (c) 2001 Avaya Global SME Solutions     */
/*                                                                  */
/*  Contents:-                                                      */
/*  IP400 Office Dev link DLL provides an interface for managing    */
/*  the IP400 Office product ranges from a Windows PC.              */
/********************************************************************/

#ifndef _DEVLINK_H_
#define _DEVLINK_H_

typedef char TEXT;

#define DEVLINK_SUCCESS				0
#define DEVLINK_UNSPECIFIEDFAIL		1
#define DEVLINK_LICENCENOTFOUND		2

#define DEVLINK_COMMS_OPERATIONAL 0
#define DEVLINK_COMMS_NORESPONSE  1
#define DEVLINK_COMMS_REJECTED    2
#define DEVLINK_COMMS_MISSEDPACKETS 3

#ifdef __cplusplus
extern "C"
{
#endif

	typedef void (CALLBACK * DLCALLLOGEVENT)(
    DWORD   pbxh,
    TEXT   * info
    );

typedef void (CALLBACK * DLCOMMSEVENT)(
    DWORD   pbxh,
    DWORD    comms_state,
    DWORD    parm1
    );

LONG  PASCAL  DLOpen( DWORD pbxh
                       , TEXT * pbx_address
                       , TEXT * pbx_password
                       , TEXT * reserved1
                       , TEXT * reserved2
                       , DLCOMMSEVENT cb
                       );
LONG  PASCAL  DLClose( DWORD pbxh );

LONG  PASCAL  DLRegisterType2CallDeltas( DWORD  pbxh, DLCALLLOGEVENT cb );

#ifdef __cplusplus
};
#endif

#endif // _DEVLINK_H_
