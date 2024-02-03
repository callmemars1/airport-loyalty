import type {TableFlightDto, AirportDto, UserDto} from './dto';

export const PageSize = 10;


export async function getDepartingFlights(atAirportIataCode: string, fromUtc?: string): Promise<TableFlightDto[]> {
    const queryParams = new URLSearchParams();
    queryParams.append('atAirportIataCode', atAirportIataCode);
    if (fromUtc) queryParams.append('fromUtc', fromUtc);

    const response = await fetch(`/api/flights/departing?${queryParams}`);
    if (!response.ok) {
        throw new Error('Failed to fetch departing flights');
    }

    return response.json();
}

export async function getArrivingFlights(atAirportIataCode: string, fromUtc?: string): Promise<TableFlightDto[]> {
    const queryParams = new URLSearchParams();
    queryParams.append('atAirportIataCode', atAirportIataCode);
    if (fromUtc) queryParams.append('fromUtc', fromUtc);

    const response = await fetch(`/api/flights/arriving?${queryParams}`);
    if (!response.ok) {
        throw new Error('Failed to fetch departing flights');
    }

    return response.json();
}


export async function getFlightsBySearchParameters(
    departureAirportCode: string,
    arrivalAirportCode: string,
    pageNumber: number,
    departureDate: Date | undefined = undefined
): Promise<TableFlightDto[]> {
    try {
        const response = await fetch(
            `/api/flights/filtered?departureAirportCode=${departureAirportCode}&arrivalAirportCode=${arrivalAirportCode}${departureDate ? '&departureDate=' + departureDate : ''}&pageNumber=${pageNumber}&pageSize=${PageSize}`
        );
        return await response.json();
    } catch (error) {
        console.error('Failed to fetch flights:', error);
        throw error
    }
}


export async function getFlightByNumber(
   flightNumber: string
): Promise<TableFlightDto[]> {
    try {
        const response = await fetch(
            `/api/flights/by-number?flightNumber=${flightNumber}`
        );
        if (response.ok)
            return [ await response.json() ]
        
        return []
    } catch (error) {
        console.error('Failed to fetch flight', error);
        throw error
    }
}

export async function getAirports(): Promise<AirportDto> {
    try {
        const response = await fetch(
            `/api/flights/airports`
        );
        return await response.json();
    } catch (error) {
        console.error('Failed to fetch airports:', error);
        throw error;
    }
}

export async function getUsers(pageNumber: number): Promise<UserDto[]> {
    try {
        const response = await fetch(
            `/api/users?pageNumber=${pageNumber}&pageSize=${PageSize}`
        );
        return await response.json();
    } catch (error) {
        console.error('Failed to fetch users:', error);
        throw error;
    }
}


export async function getUserData(): Promise<UserDto> {
    try {
        const response = await fetch(
            `/api/users/current-data`
        );
        return await response.json();
    } catch (error) {
        console.error('Failed to fetch users data', error);
        throw error;
    }
}


export async function getUserDataById(userId: string): Promise<UserDto> {
    try {
        const response = await fetch(
            `/api/users/by-id?userid=${userId}`
        );
        return await response.json();
    } catch (error) {
        console.error('Failed to fetch users data', error);
        throw error;
    }
}



export async function deleteUserById(userId: string): Promise<void> {
    try {
        await fetch(`/api/users/delete?userId=${userId}`, {
                method: 'DELETE',
            }
        );
    } catch (error) {
        console.error('Failed to fetch users data', error);
        throw error;
    }
}

export async function selfDeleteUser(): Promise<void> {
    try {
        await fetch(`/api/users/self-delete`, {
                method: 'DELETE',
            }
        );
    } catch (error) {
        console.error('Failed to fetch users data', error);
        throw error;
    }
}


export async function getRoles(): Promise<RoleDto> {
    try {
        const response = await fetch(
            `/api/users/roles`
        );
        return await response.json();
    } catch (error) {
        console.error('Failed to fetch roles:', error);
        throw error;
    }
}

export async function getAirlines(): Promise<RoleDto> {
    try {
        const response = await fetch(
            `/api/flights/airlines`
        );
        return await response.json();
    } catch (error) {
        console.error('Failed to fetch airlines:', error);
        throw error;
    }
}

export async function getServiceClassesForFlight(flightId: string): Promise<ServiceClass[]> {
    try {
        const response = await fetch(
            `/api/flights/service-classes?flightId=${flightId}&includeProducts=false`
        );
        return await response.json();
    } catch (error) {
        console.error('Failed to fetch service classes:', error);
        throw error;
    }
}


interface RoleDto {
    systemName: string
    title: string
    description: string
}

interface ServiceClass {
    id: number,
    title: string,
    rowsCount: number,
    rowsOffset: number,
    seatsPerRow: number,
    serviceLevel: number
}