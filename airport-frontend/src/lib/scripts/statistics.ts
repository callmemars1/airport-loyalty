export interface PerDayStatistic{
    date: string,
    unitsCount: number
}

export async function getUsersCountPerDateStatistics(): Promise<PerDayStatistic[]> {
    const response = await fetch(`/api/statistics/users-per-date`);
    if (response.ok) {
        let body = await response.json()
        return body.units
    }
    else {
        throw new Error('Failed to fetch /api/statistics/users-per-date');
    }
}

export async function getFlightsPerDateStatistics(): Promise<PerDayStatistic[]> {
    const response = await fetch(`/api/statistics/flights-per-date`);
    if (response.ok) {
        let body = await response.json()
        return body.units
    }
    else {
        throw new Error('Failed to fetch /api/statistics/flights-per-date');
    }
}

export async function getPurchasesPerDateStatistics(): Promise<PerDayStatistic[]> {
    const response = await fetch(`/api/statistics/purchases-per-date`);
    if (response.ok) {
        let body = await response.json()
        return body.units
    }
    else {
        throw new Error('Failed to fetch /api/statistics/purchases-per-date');
    }
}