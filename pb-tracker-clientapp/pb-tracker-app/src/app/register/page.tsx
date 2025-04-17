import { RegisterForm } from '@/components/forms/register-form';

export default function RegisterPage() {
    return (
        <div className='h-screen w-screen flex items-center justify-center bg-muted px-4'>
            <div className='w-full max-w-md sm:max-w-sm md:max-w-md lg:max-w-md'>
                <RegisterForm />
            </div>
        </div>
    );
}
