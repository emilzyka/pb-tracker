// signup-form.tsx
import { z } from 'zod';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useState } from 'react';

import {
    Form,
    FormField,
    FormItem,
    FormLabel,
    FormControl,
    FormMessage,
} from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import {
    Card,
    CardHeader,
    CardTitle,
    CardDescription,
    CardContent,
} from '@/components/ui/card';
import { cn } from '@/lib/utils';
import { Link, useNavigate } from 'react-router';
import { registerUser } from '@/api/auth/auth';

const signupFormSchema = z.object({
    username: z.string().min(3).max(50),
    password: z.string().min(6, 'Password must be at least 6 characters'),
});

type RegisterFormProps = React.ComponentProps<'div'>;

export const RegisterForm: React.FC<RegisterFormProps> = ({
    className,
    ...props
}) => {
    const form = useForm<z.infer<typeof signupFormSchema>>({
        resolver: zodResolver(signupFormSchema),
        defaultValues: {
            username: '',
            password: '',
        },
    });

    const navigate = useNavigate();
    const [error, setError] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const onSubmit = async (values: z.infer<typeof signupFormSchema>) => {
        setIsSubmitting(true);
        setError(null);

        try {
            await registerUser(values.username, values.password);
            navigate('/login');
        } catch (err) {
            console.error('Signup error:', err);
            setError('Signup failed. Please try again.');
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div className={cn('flex flex-col gap-6', className)} {...props}>
            <Card>
                <CardHeader className='text-center'>
                    <CardTitle className='text-xl'>
                        Create your account
                    </CardTitle>
                    <CardDescription>
                        Start tracking your progress today
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <Form {...form}>
                        <form
                            onSubmit={form.handleSubmit(onSubmit)}
                            className='space-y-6'>
                            {/* Username */}
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

                            {/* Password */}
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
                                                autoComplete='new-password'
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

                            {/* Submit */}
                            <Button
                                type='submit'
                                className='w-full text-white'
                                disabled={isSubmitting}>
                                {isSubmitting
                                    ? 'Creating account...'
                                    : 'Sign up'}
                            </Button>

                            <div className='text-center text-sm'>
                                Already have an account?{' '}
                                <Link to='/login' className='underline'>
                                    Login
                                </Link>
                            </div>
                        </form>
                    </Form>
                </CardContent>
            </Card>

            <div className='text-muted-foreground text-center text-xs text-balance *:[a]:underline *:[a]:underline-offset-4 *:[a]:hover:text-primary'>
                <span className='block mt-1'>
                    Please don’t store any sensitive or personal information in
                    your account.
                </span>
            </div>
        </div>
    );
};
