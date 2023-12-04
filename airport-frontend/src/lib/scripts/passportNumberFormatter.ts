export default class PassportNumberFormatter {
    private passportNumber: string;
    private onPassportNumberChange: (newPassportNumber: string) => void;

    constructor(initialPassportNumber: string, onPassportNumberChange: (newPassportNumber: string) => void) {
        this.passportNumber = initialPassportNumber;
        this.onPassportNumberChange = onPassportNumberChange;
    }

    public format = (e: InputEvent) => {
        const regexp = new RegExp(/^\d+$/)

        if (e.data && !regexp.test(e.data)) {
            e.preventDefault();
        } else if (this.passportNumber.length === 11 && e.data) {
            e.preventDefault();
        }

        if (this.passportNumber.length === 4 && e.data) {
            this.passportNumber = this.passportNumber.concat(' ');
        } else if (this.passportNumber.length === 6 && !e.data) {
            this.passportNumber = this.passportNumber.substring(0, this.passportNumber.length - 1);
        }

        // Call the closure function with the updated passport number
        // this.onPassportNumberChange(this.passportNumber);
    }
}