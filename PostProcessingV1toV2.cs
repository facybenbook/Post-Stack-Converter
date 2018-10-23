using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingV1toV2
{
    public static PostProcessProfile Convert(PostProcessingProfile original)
    {
        var ppp = ScriptableObject.CreateInstance<PostProcessProfile>();

        var ao = original.ambientOcclusion.settings;
        var aa = original.antialiasing.settings;
        var bloom = original.bloom.settings;
        var ca = original.chromaticAberration.settings;
        var cg = original.colorGrading.settings;
        var dof = original.depthOfField.settings;
        var dither = original.dithering.settings;
        var eye = original.eyeAdaptation.settings;
        var fog = original.fog.settings;
        var grain = original.grain.settings;
        var mb = original.motionBlur.settings;
        var ssr = original.screenSpaceReflection.settings;
        var lut = original.userLut.settings;
        var vignette = original.vignette.settings;

        // LUT is complex. Only works in ColorGrading if Mode is 'LDR' or 'External'
        // Not -quite- sure how to handle this (a new custom effect?)

        if (original.vignette.enabled)
            ConvertVignetteSettings(ppp, vignette);

        if (original.screenSpaceReflection.enabled)
            ConvertReflectionSettings(ppp, ssr);

        if (original.motionBlur.enabled)
            ConvertMotionBlurSettings(ppp, mb);

        if (original.grain.enabled)
            ConvertGrainSettings(ppp, grain);

        // Skipping fog - moved to another part of the profile.

        if (original.chromaticAberration.enabled)
            ConvertChromaticAberrationSettings(ppp, ca);

        if (original.depthOfField.enabled)
            ConvertDepthOfFieldSettings(ppp, dof);

        // Skipping dithering (no matching equivialent?)

        if (original.eyeAdaptation.enabled)
            ConvertEyeAdaptionSettings(ppp, eye);

        if (original.ambientOcclusion.enabled)
            ConvertAmbientOcclusionSettings(ppp, ao);

        // Skipping Anti-aliasing (rightfully this shouldn't be part of a profile.)

        if (original.bloom.enabled)
            ConvertBloomSettings(ppp, bloom);

        if (original.colorGrading.enabled)
            ConvertColourGradingSettings(ppp, cg);

        return ppp;
    }

    private static void ConvertVignetteSettings(PostProcessProfile ppp, VignetteModel.Settings vignette)
    {
        var vignette2 = ppp.AddSettings<Vignette>();

        vignette2.center.overrideState = true;
        vignette2.center.value = vignette.center;

        vignette2.color.overrideState = true;
        vignette2.color.value = vignette.color;

        vignette2.intensity.overrideState = true;
        vignette2.intensity.value = vignette.intensity;

        vignette2.mask.overrideState = true;
        vignette2.mask.value = vignette.mask;

        vignette2.mode.overrideState = true;
        switch (vignette.mode)
        {
            case VignetteModel.Mode.Classic:
                vignette2.mode.value = VignetteMode.Classic;
                break;
            case VignetteModel.Mode.Masked:
                vignette2.mode.value = VignetteMode.Masked;
                break;
        }
        vignette2.opacity.overrideState = true;
        vignette2.opacity.value = vignette.opacity;
        vignette2.rounded.overrideState = true;
        vignette2.rounded.value = vignette.rounded;
        vignette2.roundness.overrideState = true;
        vignette2.roundness.value = vignette.roundness;
        vignette2.smoothness.overrideState = true;
        vignette2.smoothness.value = vignette.smoothness;
    }

    private static void ConvertReflectionSettings(PostProcessProfile ppp, ScreenSpaceReflectionModel.Settings ssr)
    {
        var ssr2 = ppp.AddSettings<ScreenSpaceReflections>();
        ssr2.SetAllOverridesTo(true);
        var intensity = ssr.intensity;
        ssr2.distanceFade.value = intensity.fadeDistance;
        // Not supported: intensity.fresnelFade;
        // Not supported: intensity.fresnelFadePower;
        // Not supported: intensity.reflectionMultiplier;

        var reflection = ssr.reflection;
        //reflection.blendType;
        ssr2.maximumIterationCount.value = reflection.iterationCount;
        ssr2.maximumMarchDistance.value = reflection.maxDistance;
        //reflection.reflectBackfaces;
        //reflection.reflectionBlur;
        switch (reflection.reflectionQuality)
        {
            case ScreenSpaceReflectionModel.SSRResolution.High:
                ssr2.resolution.value = ScreenSpaceReflectionResolution.FullSize;
                break;
            case ScreenSpaceReflectionModel.SSRResolution.Low:
                ssr2.resolution.value = ScreenSpaceReflectionResolution.Downsampled;
                break;
        }
        //reflection.stepSize;
        ssr2.thickness.value = reflection.widthModifier;

        var mask = ssr.screenEdgeMask;
        ssr2.vignette.value = mask.intensity;
    }

    private static void ConvertMotionBlurSettings(PostProcessProfile ppp, MotionBlurModel.Settings mb)
    {
        var mb2 = ppp.AddSettings<MotionBlur>();
        mb2.sampleCount.overrideState = true;
        mb2.sampleCount.value = mb.sampleCount;
        mb2.shutterAngle.overrideState = true;
        mb2.shutterAngle.value = mb.shutterAngle;
        // Not supported: mb.frameBlending
    }

    private static void ConvertGrainSettings(PostProcessProfile ppp, GrainModel.Settings grain)
    {
        var grain2 = ppp.AddSettings<Grain>();
        grain2.colored.overrideState = true;
        grain2.colored.value = grain.colored;
        grain2.intensity.overrideState = true;
        grain2.intensity.value = grain.intensity;
        grain2.lumContrib.overrideState = true;
        grain2.lumContrib.value = grain.luminanceContribution;
        grain2.size.overrideState = true;
        grain2.size.value = grain.size;
    }

    private static void ConvertChromaticAberrationSettings(PostProcessProfile ppp, ChromaticAberrationModel.Settings ca)
    {
        var ca2 = ppp.AddSettings<ChromaticAberration>();
        ca2.intensity.overrideState = true;
        ca2.intensity.value = ca.intensity;
        ca2.spectralLut.overrideState = true;
        ca2.spectralLut.value = ca.spectralTexture;
    }

    private static void ConvertDepthOfFieldSettings(PostProcessProfile ppp, DepthOfFieldModel.Settings dof)
    {
        var dof2 = ppp.AddSettings<DepthOfField>();
        dof2.aperture.overrideState = true;
        dof2.aperture.value = dof.aperture;
        dof2.focalLength.overrideState = true;
        dof2.focalLength.value = dof.focalLength;
        dof2.focusDistance.overrideState = true;
        dof2.focusDistance.value = dof.focusDistance;
        dof2.kernelSize.overrideState = true;
        switch (dof.kernelSize)
        {
            case DepthOfFieldModel.KernelSize.VeryLarge:
                dof2.kernelSize.value = KernelSize.VeryLarge;
                break;
            case DepthOfFieldModel.KernelSize.Large:
                dof2.kernelSize.value = KernelSize.Large;
                break;
            case DepthOfFieldModel.KernelSize.Medium:
                dof2.kernelSize.value = KernelSize.Medium;
                break;
            case DepthOfFieldModel.KernelSize.Small:
                dof2.kernelSize.value = KernelSize.Small;
                break;
        }
        // Not supported: dof.useCameraFov;
    }

    private static void ConvertEyeAdaptionSettings(PostProcessProfile ppp, EyeAdaptationModel.Settings eye)
    {
        var eye2 = ppp.AddSettings<AutoExposure>();
        eye2.eyeAdaptation.overrideState = true;
        eye2.eyeAdaptation.value = eye.adaptationType == EyeAdaptationModel.EyeAdaptationType.Fixed
            ? EyeAdaptation.Fixed
            : EyeAdaptation.Progressive;

        eye2.keyValue.overrideState = true;
        eye2.keyValue.value = eye.keyValue;
        // Not supported: eye.dynamicKeyValue;
        //                eye.highPercent;
        //                eye.logMax;
        //                eye.logMin;
        //                eye.lowPercent;
        eye2.maxLuminance.overrideState = true;
        eye2.maxLuminance.value = eye.maxLuminance;
        eye2.minLuminance.overrideState = true;
        eye2.minLuminance.value = eye.minLuminance;
        eye2.speedDown.overrideState = true;
        eye2.speedDown.value = eye.speedDown;
        eye2.speedUp.overrideState = true;
        eye2.speedUp.value = eye.speedUp;
    }

    private static void ConvertBloomSettings(PostProcessProfile ppp, BloomModel.Settings bloom)
    {
        var bloom2 = ppp.AddSettings<Bloom>();

        var bloomSettings = bloom.bloom;
        // Skipping bloomSettings.antiFlicker;
        bloom2.intensity.overrideState = true;
        bloom2.intensity.value = bloomSettings.intensity;
        bloom2.diffusion.overrideState = true;
        bloom2.diffusion.value = bloomSettings.radius; // TODO: Unsure this is the right setting
        bloom2.softKnee.overrideState = true;
        bloom2.softKnee.value = bloomSettings.softKnee;
        bloom2.threshold.overrideState = true;
        bloom2.threshold.value = bloomSettings.threshold;

        var dirt = bloom.lensDirt;
        bloom2.dirtIntensity.overrideState = true;
        bloom2.dirtIntensity.value = dirt.intensity;
        bloom2.dirtTexture.overrideState = true;
        bloom2.dirtTexture.value = dirt.texture;
    }

    private static void ConvertAmbientOcclusionSettings(PostProcessProfile ppp, AmbientOcclusionModel.Settings oldAmbientOcclusionSettings)
    {
        var newAmbientOcclusionSettings = ppp.AddSettings<AmbientOcclusion>();
        newAmbientOcclusionSettings.ambientOnly.overrideState = true;
        newAmbientOcclusionSettings.ambientOnly.value = oldAmbientOcclusionSettings.ambientOnly;
        // Ignoring ao.downsampling
        // Ignoring ao.forceForwardCompatibility
        newAmbientOcclusionSettings.intensity.overrideState = true;
        newAmbientOcclusionSettings.intensity.value = oldAmbientOcclusionSettings.intensity;

        newAmbientOcclusionSettings.quality.overrideState = true;
        switch (oldAmbientOcclusionSettings.sampleCount)
        {
            case AmbientOcclusionModel.SampleCount.High:
                newAmbientOcclusionSettings.quality.value = AmbientOcclusionQuality.High;
                break;
            case AmbientOcclusionModel.SampleCount.Medium:
                newAmbientOcclusionSettings.quality.value = AmbientOcclusionQuality.Medium;
                break;
            case AmbientOcclusionModel.SampleCount.Low:
                newAmbientOcclusionSettings.quality.value = AmbientOcclusionQuality.Low;
                break;
            case AmbientOcclusionModel.SampleCount.Lowest:
                newAmbientOcclusionSettings.quality.value = AmbientOcclusionQuality.Lowest;
                break;
        }

        newAmbientOcclusionSettings.radius.overrideState = true;
        newAmbientOcclusionSettings.radius.value = oldAmbientOcclusionSettings.radius;
        // Ignoring ao.highPrecision
    }

    private static void ConvertColourGradingSettings(PostProcessProfile ppp, ColorGradingModel.Settings oldColorGradingSettings)
    {
        var newColorGradingSettings = ppp.AddSettings<ColorGrading>();
        newColorGradingSettings.SetAllOverridesTo(true);

        // Basic Settings
        var oldBasicSettings = oldColorGradingSettings.basic;
        newColorGradingSettings.postExposure.value = oldBasicSettings.postExposure;
        newColorGradingSettings.contrast.value = oldBasicSettings.contrast;
        newColorGradingSettings.hueShift.value = oldBasicSettings.hueShift;
        newColorGradingSettings.saturation.value = oldBasicSettings.saturation;
        newColorGradingSettings.temperature.value = oldBasicSettings.temperature;
        newColorGradingSettings.tint.value = oldBasicSettings.tint;

        // Mixer Settings
        var oldColorMixer = oldColorGradingSettings.channelMixer;
        newColorGradingSettings.mixerBlueOutRedIn.value = oldColorMixer.red.z;
        newColorGradingSettings.mixerBlueOutGreenIn.value = oldColorMixer.green.z;
        newColorGradingSettings.mixerBlueOutBlueIn.value = oldColorMixer.blue.z;
        newColorGradingSettings.mixerGreenOutRedIn.value = oldColorMixer.red.y;
        newColorGradingSettings.mixerGreenOutGreenIn.value = oldColorMixer.green.y;
        newColorGradingSettings.mixerGreenOutBlueIn.value = oldColorMixer.blue.y;
        newColorGradingSettings.mixerRedOutRedIn.value = oldColorMixer.red.x;
        newColorGradingSettings.mixerRedOutGreenIn.value = oldColorMixer.green.x;
        newColorGradingSettings.mixerRedOutBlueIn.value = oldColorMixer.blue.x;

        // Curves
        var oldColorCurves = oldColorGradingSettings.curves;
        newColorGradingSettings.blueCurve.value.curve = oldColorCurves.blue.curve;
        newColorGradingSettings.greenCurve.value.curve = oldColorCurves.green.curve;
        newColorGradingSettings.redCurve.value.curve = oldColorCurves.red.curve;
        newColorGradingSettings.hueVsHueCurve.value.curve = oldColorCurves.hueVShue.curve;
        newColorGradingSettings.hueVsSatCurve.value.curve = oldColorCurves.hueVSsat.curve;
        newColorGradingSettings.lumVsSatCurve.value.curve = oldColorCurves.lumVSsat.curve;
        newColorGradingSettings.satVsSatCurve.value.curve = oldColorCurves.satVSsat.curve;
        newColorGradingSettings.masterCurve.value.curve = oldColorCurves.master.curve;

        // Tone mapping
        var oldTonemapper = oldColorGradingSettings.tonemapping;
        switch (oldTonemapper.tonemapper)
        {
            case ColorGradingModel.Tonemapper.None:
                newColorGradingSettings.tonemapper.value = Tonemapper.None;
                break;
            case ColorGradingModel.Tonemapper.ACES:
                newColorGradingSettings.tonemapper.value = Tonemapper.ACES;
                break;
            case ColorGradingModel.Tonemapper.Neutral:
                newColorGradingSettings.tonemapper.value = Tonemapper.Neutral;
                break;
        }

        /*
         * These settings might be possible via the custom curve options.
         * but have no direct mapping in the new post stack.
               - tonemap.neutralBlackIn;
               - tonemap.neutralBlackOut;
               - tonemap.neutralWhiteClip;
               - tonemap.neutralWhiteIn;
               - tonemap.neutralWhiteLevel;
               - tonemap.neutralWhiteOut;

          * ColorGradingModelEditor.cs:280 seems to describe how to do this.
        */

        var oldColorWheels = oldColorGradingSettings.colorWheels;
        newColorGradingSettings.gain.value = oldColorWheels.linear.gain;
        newColorGradingSettings.gamma.value = oldColorWheels.linear.gamma;
        newColorGradingSettings.lift.value = oldColorWheels.linear.lift;

        /* These also have no mapping in the new stack 
                - color.log.offset;
                - color.log.power;
                - color.log.slope;
        */
    }
}