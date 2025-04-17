import { cn } from '@/lib/utils';
import { Button } from '@/components/ui/button';
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { z } from 'zod';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { Link, useNavigate } from 'react-router';
import { useState } from 'react';
import {
    Form,
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
} from '../ui/form';
import { loginUser } from '@/api/auth/auth';

const loginFormSchema = z.object({
    username: z.string().min(3).max(50),
    password: z.string().min(3).max(50),
});

type LoginFormProps = React.ComponentProps<'div'>;

export const LoginForm: React.FC<LoginFormProps> = ({
    className,
    ...props
}) => {
    const form = useForm<z.infer<typeof loginFormSchema>>({
        resolver: zodResolver(loginFormSchema),
        defaultValues: {
            username: '',
            password: '',
        },
    });

    const navigate = useNavigate();
    const [error, setError] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState<boolean>(false);

    const onSubmit = async (values: z.infer<typeof loginFormSchema>) => {
        setIsSubmitting(true);
        setError(null);

        try {
            await loginUser(values.username, values.password);
            navigate('/');
        } catch (error) {
            setError(
                'Login failed. Please check your credentials and try again.'
            );
            console.error('Login error:', error);
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div className={cn('flex flex-col gap-6', className)} {...props}>
            <Card>
                <CardHeader className='text-center'>
                    <CardTitle className='text-xl'>Welcome back</CardTitle>
                    <CardDescription>
                        Track your personal bests and progress over time
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <Form {...form}>
                        <form
                            onSubmit={form.handleSubmit(onSubmit)}
                            className='space-y-6'>
                            {/* Username Field */}
                            <FormField
                                control={form.control}
                                name='username'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Username</FormLabel>
                                        <FormControl>
                                            <Input
                                                placeholder='username'
                                                {...field}
                                            />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />

                            {/* Password Field */}
                            <FormField
                                control={form.control}
                                name='password'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Password</FormLabel>
                                        <FormControl>
                                            <Input
                                                type='password'
                                                placeholder='••••••••'
                                                {...field}
                                            />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />

                            {/* Error message */}
                            {error && (
                                <p className='text-sm text-red-500'>{error}</p>
                            )}

                            {/* Submit Button */}
                            <Button
                                type='submit'
                                className='w-full text-primary'
                                disabled={isSubmitting}>
                                {isSubmitting ? 'Logging in...' : 'Login'}
                            </Button>

                            {/* Sign up link */}
                            <div className='text-center text-sm'>
                                Don&apos;t have an account?{' '}
                                <Link to='/register' className='underline'>
                                    Sign up
                                </Link>
                            </div>
                        </form>
                    </Form>
                </CardContent>
            </Card>

            {/* Footer message */}
            <div className='text-muted-foreground text-center text-xs text-balance *:[a]:underline *:[a]:underline-offset-4 *:[a]:hover:text-primary'>
                <span className='block mt-1'>
                    Please don’t store any sensitive or personal information in
                    your account.
                </span>
            </div>
        </div>
    );
};
