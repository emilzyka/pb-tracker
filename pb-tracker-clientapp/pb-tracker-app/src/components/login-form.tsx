import { cn } from '@/lib/utils';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { z } from 'zod';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';

const loginFormSchema = z.object({
    username: z.string().min(2).max(50),
    password: z.string().min(2).max(50),
});

type LoginFormProps = React.ComponentProps<'div'>;

export const LoginForm: React.FC<LoginFormProps> = ({ className, ...props }) => {
    const form = useForm<z.infer<typeof loginFormSchema>>({
        resolver: zodResolver(loginFormSchema),
        defaultValues: {
            username: '',
            password: '',
        },
    });

    return (
        <div className={cn('flex flex-col gap-6', className)} {...props}>
            <Card>
                <CardHeader className='text-center'>
                    <CardTitle className='text-xl'>Welcome back</CardTitle>
                    <CardDescription>Track your personal bests and progress over time</CardDescription>
                </CardHeader>
                <CardContent>
                    <form>
                        <div className='grid gap-6'>
                            <div className='after:border-border relative text-center text-sm after:absolute after:inset-0 after:top-1/2 after:z-0 after:flex after:items-center after:border-t'>
                                <span className='bg-card text-muted-foreground relative z-10 px-2'>Login</span>
                            </div>
                            <div className='grid gap-6'>
                                <div className='grid gap-3'>
                                    <Label htmlFor='email'>Username</Label>
                                    <Input id='email' type='email' placeholder='m@example.com' required />
                                </div>
                                <div className='grid gap-3'>
                                    <div className='flex items-center'>
                                        <Label htmlFor='password'>Password</Label>
                                        <a href='#' className='ml-auto text-sm underline-offset-4 hover:underline'>
                                            Forgot your password?
                                        </a>
                                    </div>
                                    <Input id='password' type='password' required />
                                </div>
                                <Button type='submit' className='w-full text-white'>
                                    Login
                                </Button>
                            </div>
                            <div className='text-center text-sm'>
                                Don&apos;t have an account?{' '}
                                <a href='#' className='underline underline-offset-4'>
                                    Sign up
                                </a>
                            </div>
                        </div>
                    </form>
                </CardContent>
            </Card>
            <div className='text-muted-foreground text-center text-xs text-balance *:[a]:underline *:[a]:underline-offset-4 *:[a]:hover:text-primary'>
                <span className='block mt-1'>Please don’t store any sensitive or personal information in your account.</span>
            </div>
        </div>
    );
};
