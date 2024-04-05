export class UserRegisterDto {
    constructor(public userName: string = "",
        public password: string = "", 
        public confirmPassword: string = ""
     ) {}
}
