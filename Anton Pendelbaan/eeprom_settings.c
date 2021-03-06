#include "Main.h"
#include "io.h"
#include <xc.h>
#include "api.h"
#include "eeprom_settings.h"
#include "api.h"
#include <stdio.h>
#include <stdlib.h>

#define LIST_SIZE 18

const unsigned int ApiList[] = {			// List containing the writable variables that require storage in EEPROM, should be smaller then 255
	TRAIN_WAIT_TIME,       					// 00         
	JUNCTION_WAIT_TIME,    					// 01
	LIGHTS_ON_WAIT_TIME,                    // 02
	STATIONARY_LEFT,                        // 03
	STATIONARY_RIGHT,                       // 04
	MAX_PWM_RIGHT,                          // 05
	MAX_PWM_LEFT,                           // 06
	MAX_JERK_PWM_BRAKE,                     // 07
	MAX_JERK_PWM,                           // 08
	INPUT_DEBOUNCE,                         // 09
	MAX_PWM_RMU_LEFT,                       // 10
	MAX_PWM_RMD_RIGHT,         	            // 11
	MAX_PWM_LMU_RIGHT,	                    // 12
	MAX_PWM_LMD_LEFT,		                // 13
	DELAY_RMU_DOWN,			                // 14
	DELAY_RMD_UP,	                        // 15
	DELAY_LMD_UP,		                    // 16
	DELAY_LMU_DOWN		                    // 17
		                                    // 18
											// 19
											// 20
											// 21
											// 22
											// 23
											// 24
											// 25
											// 26
											// 27
};

unsigned int ReadActive = 0;
unsigned int WriteActive = 0;

/******************************************************************************
 * Function:        EEPROMxREAD(void)
 *                  Read program values in EEPROM at startup
 *
 * PreCondition:    None
 *
 * Input:           None
 *
 * Output:          None
 *
 * Side Effects:    Initialize program variables from EEPROM
 *
 * Overview:        None
 *****************************************************************************/
unsigned int EEPROMxREAD(void)
{
	unsigned char i = 0;
	unsigned int Data = 0;
#ifdef DEBUG    
    printf("\r\n-----------------------------------------------------------------\r\n");
#endif

    if (!WriteActive){
        ReadActive = On;
        INTCON = 0x00;
    
        for(i = 0; i < LIST_SIZE; i++ )
        {        
        	Data = Eeprom_Read(i);							// read the data on the location according to the variables numbered in the ApiList
#ifdef DEBUG  
        printf("index: %d Data: 0x%X\r\n", i, Data);
#endif
        	API[ApiList[i]] = Data;							// Store the data in API to be used by the program	
        	API_EEPROM[ApiList[i]] = Data;					// Store the data in API_EEPROM to be used for determining update of EEPROM val in EEPROMxSTORE()
        	// temporary rule as long as EEPROM is not configured yet --> to be deleted and above 2 lines uncommented
        }
    
        INTCON = 0xA0;
        ReadActive = Off;
        return (On);
    }
    else{
        return (Off);
    }
    
}

/******************************************************************************
 * Function:        unsigned int Eeprom_Read(unsigned int Location)
 *                  Read program values in EEPROM
 *
 * PreCondition:    None
 *
 * Input:           None
 *
 * Output:          None
 *
 * Side Effects:    Reads vital program variables from EEPROM
 *
 * Overview:        None
 *****************************************************************************/
unsigned int Eeprom_Read(unsigned int Location)
{
	unsigned int Location_High_Byte = Location * 2;
	unsigned int Location_Low_Byte  = Location_High_Byte + 1;
	unsigned int Return_Data = 0;
	
    //INTCON = 0x00;
    
	EECON1bits.EEPGD = 0;           //access eeprom
    EECON1bits.CFGS = 0;            //access eeprom
	EEADR = Location_High_Byte;           
	EECON1bits.RD = 1; 
    while (EECON1bits.RD);
	Return_Data = EEDATA;  
	Return_Data = Return_Data << 8;
#ifdef DEBUG
    printf("Location_High_Byte %d EEADR: 0x%X EEDATA: 0x%X\r\n", Location_High_Byte, EEADR, EEDATA);
#endif
    
    EECON1bits.EEPGD = 0;           //access eeprom
    EECON1bits.CFGS = 0;            //access eeprom
	EEADR = Location_Low_Byte;           
	EECON1bits.RD = 1; 
    while (EECON1bits.RD);
	Return_Data = Return_Data | EEDATA;
#ifdef DEBUG
    printf("Location_Low_Byte %d EEADR: 0x%X EEDATA: 0x%X\r\n", Location_Low_Byte, EEADR, EEDATA);
#endif

    EEADR = 0xFF;                                                               // point EEADR to not used location to prevent rpurious write on used addresses
    
    //INTCON = 0xA0;
	
	return (Return_Data);
}

/******************************************************************************
 * Function:        EEPROMxSTORE(void)
 *                  Store program values in EEPROM if a value is changed
 *
 * PreCondition:    None
 *
 * Input:           None
 *
 * Output:          None
 *
 * Side Effects:    Stores vital program variables in EEPROM
 *
 * Overview:        None
 *****************************************************************************/
unsigned int EEPROMxSTORE(void)
{
	unsigned char i, api_list;
    
    if (!ReadActive){
        WriteActive = On;
        INTCON = 0x00;
	
        for(i = 0; i < LIST_SIZE; i++ )
        {
            api_list = ApiList[i];
        
#ifdef DEBUG  
        printf("index outside if: 0x%X api_list: 0x%X\r\n", i, api_list);
#endif
        
        	if (API[ApiList[i]] != API_EEPROM[ApiList[i]])	// compare the data on the location according to the variables numbered in the ApiList
        	{
            
#ifdef DEBUG  
        printf("index inside if: 0x%X api_list: 0x%X\r\n", i, api_list);
#endif
        		Eeprom_Store(i, API[ApiList[i]]); 			// Send list index number as Location and the API[address].value to be stored.
        		API_EEPROM[ApiList[i]] = API[ApiList[i]];    // Store the new value also in te API_EEPROM for next comparisson
        	}
        }
    
        INTCON = 0xA0;
        WriteActive = Off;
        return (On);
    }
    else{
        return (Off);
    }    
}

/******************************************************************************
 * Function:        Eeprom_Store(unsigned int Location, unsigned int Value)
 *                  Store program values in EEPROM if a value is changed
 *
 * PreCondition:    None
 *
 * Input:           None
 *
 * Output:          None
 *
 * Side Effects:    Stores vital program variables in EEPROM
 *
 * Overview:        None
 *****************************************************************************/
void Eeprom_Store(unsigned int Location, unsigned int Value)
{	
	unsigned int Location_High_Byte = Location * 2;
	unsigned int Location_Low_Byte  = Location_High_Byte + 1;

    //INTCON = 0x00;                  // disable interrupts
    
    EECON1bits.WRERR = 0;
	EEADR = (unsigned char)Location_High_Byte;
	EEDATA = (unsigned char)(Value >> 8);
    EECON1bits.EEPGD = 0;           //access eeprom
    EECON1bits.CFGS = 0;            //access eeprom
    EECON1bits.WREN = 1;            //write enable eeprom    
	EECON2 = 0x55;
	EECON2 = 0xAA;
    EECON1bits.WR = 1;
	while (EECON1bits.WR && !PIR2bits.EEIF){
        continue;
    }
    PIR2bits.EEIF = 0;
#ifdef DEBUG
    if (EECON1bits.WRERR){
        printf("WRERR bit high:Location_High_Byte %d Value: 0x%X EEADR: 0x%X EEDATA: 0x%X\r\n", Location_High_Byte, (Value >> 8), EEADR, EEDATA);
    }
    else{
        printf("Location_High_Byte %d Value: 0x%X EEADR: 0x%X EEDATA: 0x%X\r\n", Location_High_Byte, Value, EEADR, EEDATA);
    }
#endif
    
    EECON1bits.WRERR = 0;
	EEADR = (unsigned char)Location_Low_Byte;
	EEDATA = (unsigned char)Value;
    EECON1bits.EEPGD = 0;           //access eeprom
    EECON1bits.CFGS = 0;            //access eeprom
    EECON1bits.WREN = 1;            //write enable eeprom
    INTCON = 0x00;                  // disable interrupts
	EECON2 = 0x55;
	EECON2 = 0xAA;    
	EECON1bits.WR = 1;
	while (EECON1bits.WR && !PIR2bits.EEIF){
        continue;
    }
    PIR2bits.EEIF = 0;
#ifdef DEBUG
    if (EECON1bits.WRERR){
        printf("WRERR bit high:Location_Low_Byte %d Value: 0x%X EEADR: 0x%X EEDATA: 0x%X\r\n", Location_Low_Byte, Value, EEADR, EEDATA);
    }
    else{
        printf("Location_Low_Byte %d Value: 0x%X EEADR: 0x%X EEDATA: 0x%X\r\n", Location_Low_Byte, Value, EEADR, EEDATA);
    }
#endif
    
    EECON1bits.WREN = 0;
    
    EEADR = 0xFF;                                                               // point EEADR to not used location to prevent rpurious write on used addresses
	//INTCON = 0xA0;
}
/*	Location	Location_High_Byte				Location_Low_Byte
	0 > 		0 					and 		1
										
	1 > 		2 					and 		3
										
	2 > 		4 					and 		5
										
	3 > 		6 					and 		7
										
	4 > 		8 					and 		9
										
	5 > 		10					and			11
*/