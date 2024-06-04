export class UserRegisterDto {
    constructor(public userName: string = "",
        public password: string = "", 
        public confirmPassword: string = "",
        public gender: string = "", 
        public displayName: string = "",
        public dateOfBirth: Date,
        public city: string = "",
        public country: string = ""
     ) {}
}
