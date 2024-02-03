
import type { RequestEvent } from "@sveltejs/kit"

export let formatTwoDigit = (num: number) => {
    if (num % 10 === num)
        return `0${num}`
    return num.toString()
}

export let formatDateTime = (date: Date) => {
    if (isNaN(date.getTime()))
        return 'Не распознано'
        
    return `${formatTwoDigit(date.getHours())}:${formatTwoDigit(date.getMinutes())} — ${formatTwoDigit(date.getDate())}.${formatTwoDigit(date.getMonth() + 1)}.${date.getFullYear()}`
}

export function handleLoginRedirect(
    event: RequestEvent,
    message: string = "Сейчас у вас нет доступа к данной странице. Войдите, чтобы получить доступ!"
) {
    const redirectTo = event.url.pathname + event.url.search
    return `/auth/sign-in?redirectTo=${redirectTo}&message=${message}`
}