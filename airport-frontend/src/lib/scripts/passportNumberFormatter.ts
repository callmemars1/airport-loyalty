export default class PassportNumberFormatter {
    private passportNumber: string;
    private onPassportNumberChange: (newPassportNumber: string) => void;

    constructor(initialPassportNumber: string, onPassportNumberChange: (newPassportNumber: string) => void) {
        this.passportNumber = initialPassportNumber;
        this.onPassportNumberChange = onPassportNumberChange;
    }

    public format = (e: InputEvent) => {
        const regexp = new RegExp(/^\d+$/)

        e.preventDefault();
        if (e.data && !regexp.test(e.data)) {
            return
        } else if (this.passportNumber.length === 11 && e.data) {
            return;
        }

        if (this.passportNumber.length === 4 && e.data) {
            this.passportNumber = this.passportNumber.concat(' ');
        } else if (this.passportNumber.length === 6 && !e.data) {
            this.passportNumber = this.passportNumber.substring(0, this.passportNumber.length - 1);
        }
        
        if (e.data)
            this.passportNumber = this.passportNumber + e.data
        else
            this.passportNumber = this.passportNumber.substring(0, this.passportNumber.length - 1);
        
        // Call the closure function with the updated passport number
        this.onPassportNumberChange(this.passportNumber);
    }
}